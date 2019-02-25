using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    [SerializeField]
    private float GrabbingBodyConnectionFrequency = 4.0f;
    [SerializeField]
    private float StandardBodyConnectionFrequency = 2.0f;
    [SerializeField]
    private Vector3 IdlePositionOffset;
    [SerializeField]
    private KeyCode handInputCode;
    [SerializeField]
    private Vector3 RotatingStartPosition = new Vector3(0,0,0);
    [SerializeField]
    private int direction;

    private bool canGrab = false;
    private bool _isGrabbing = false;
    public bool isGrabbing
    {
        get { return _isGrabbing; }
        private set { _isGrabbing = value; }
    }

    private GameObject _GrabbedObject;
    public GameObject GrabbedObject
    {
        get { return _GrabbedObject; }
        private set { _GrabbedObject = value; }
    }

    private GameObject UnderlyingObject;
    private Rigidbody2D Rigidbody;
    private SpringJoint2D BodyConnection;
    private HingeJoint2D additionalJoint;
    private Vector3 collisionPoint;
    private StateMachine stateMachine;

	// Use this for initialization
	void Awake () {

        UnderlyingObject = null;
        Rigidbody = GetComponent<Rigidbody2D>();
        BodyConnection = GetComponent<SpringJoint2D>();
        stateMachine = GetComponentInParent<StateMachine>();
    }
	
	// Update is called once per frame
	void Update () {

        UpdateFreeJoint();

    }

    #region private_methods

    private void UpdateFreeJoint()
    {
        if (!isGrabbing)
            return;

        var joint = GrabbedObject.GetComponent<HingeJoint2D>();
        if (joint.connectedBody == null)
        {
            joint.connectedBody = this.Rigidbody;
            Destroy(additionalJoint);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.UnderlyingObject = collision.gameObject;

        if(collision.gameObject.tag == "Handle")
        {
            this.canGrab = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Handle")
        {
        }    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.UnderlyingObject = null;
        if(collision.gameObject.tag == "Handle")
        {
            this.canGrab = false;
        }
    }

    private void Grab(GameObject obj)
    {
        var joint = obj.GetComponent<HingeJoint2D>();
        if(joint != null)
        {
            Vector2 GrabPoint = transform.parent.transform.TransformPoint(transform.localPosition) - obj.transform.position;

            if (joint.connectedBody == null)
            {
                joint.connectedBody = this.transform.GetComponent<Rigidbody2D>();
                joint.anchor = GrabPoint;
            }
            else
            {
                additionalJoint = obj.AddComponent<HingeJoint2D>();
                additionalJoint.connectedBody = this.transform.GetComponent<Rigidbody2D>();
                additionalJoint.autoConfigureConnectedAnchor = false;
                additionalJoint.connectedAnchor = Vector2.zero;
                joint.anchor = new Vector2(-0.05f, joint.anchor.y);
                additionalJoint.anchor = new Vector2(0.05f, joint.anchor.y);
            }

            this.SetAsDynamic();
            this.AdjustBodyConnection(GrabbingBodyConnectionFrequency);
            this.GrabbedObject = obj;
            this.isGrabbing = true;

            var body = GameObject.FindGameObjectWithTag("Body");
            GameObject.FindObjectOfType<GUIController>().UpdateAltitude(body.transform.localPosition.y + 1);
        }
    }

    private void Release(GameObject obj)
    {
        if(additionalJoint == null)
        {
            var joint = obj.GetComponent<Joint2D>();
            joint.connectedBody = null;
        }
        else
        {
            additionalJoint.connectedBody = null;
            Destroy(additionalJoint);
        }
        this.AdjustBodyConnection(StandardBodyConnectionFrequency);
    }

    private void StopRotation()
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
        
    }

    private void AdjustBodyConnection(float frequency)
    {
        BodyConnection.frequency = frequency;
    }

    private IEnumerator MoveTo(Transform body)
    {
        var positionToMove = body.localPosition + RotatingStartPosition;
        while (transform.localPosition != positionToMove && stateMachine.currentState == StateMachine.PlayerStates.Rotating)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, positionToMove, Time.deltaTime * 2.0f);
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator RotateAround(Transform body, bool isContinued)
    {
        float angle = 45.0f;
        float period = .5f;
        float time = 0.0f;

        if(!isContinued)
        {
            yield return StartCoroutine(MoveTo(body));
            angle = 90.0f;
        }

        while (stateMachine.currentState == StateMachine.PlayerStates.Rotating)
        {
            time += Time.deltaTime;
            float phase = Mathf.Sin(time / period);
            transform.RotateAround(body.localPosition / 2.0f, Vector3.forward, (1 / period) * Time.deltaTime * angle * phase * direction);
            //Debug.DrawLine(Vector3.zero, body.localPosition / 2);
            yield return null;
        }
    }

    #endregion

    #region public_methods

    public bool HandleGrab()
    {
        if (Input.GetKey(handInputCode))
        {
            if (!this.isGrabbing && this.UnderlyingObject != null)
            {
                this.Grab(UnderlyingObject);
            }
            return true;
        }

        if(!Input.GetKey(handInputCode))
        {
            if (this.isGrabbing)
            {
                this.Release(this.GrabbedObject);
                this.GrabbedObject = null;
                this.isGrabbing = false;
            }
        }
        return false;
    }

    public void SetAsDynamic()
    {
        this.StopRotation();
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void SetAsKinematic()
    {
        this.StopRotation();
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetAsIdle(Transform body)
    {
        this.SetAsKinematic();
        this.transform.localPosition = body.localPosition + IdlePositionOffset;
        this.transform.SetParent(body);
        this.GetComponent<Joint2D>().enabled = false;
    }

    public void SetAsActive(Transform body)
    {
        this.SetAsDynamic();
        this.transform.SetParent(body);
        this.GetComponent<Joint2D>().enabled = true;
    }

    public void StartRotating(Transform body, bool isContinued)
    {
        this.SetAsKinematic();
        StartCoroutine(RotateAround(body, isContinued));
    }

    public void DisableJointCollisions()
    {
        this.GetComponent<Joint2D>().enableCollision = false;
    }

    public void EnableJointCollisions()
    {
        this.GetComponent<Joint2D>().enableCollision = true;
    }

    public void Attach(GameObject obj)
    {
        this.Grab(obj);
    }

    public void BodyJointSetEnabled(bool enabled)
    {
        this.GetComponent<Joint2D>().enabled = enabled;
    }

    #endregion
}
