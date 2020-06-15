using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour 
{
    private Vector3 lastLocalPosition;
    private Vector3 _localPositionDelta;
    public Vector3 localPositionDelta
    {
        get{ return _localPositionDelta; }
        private set { _localPositionDelta = value; }
    }

    private void Awake()
    {
		lastLocalPosition = transform.localPosition;
    }

    private void FixedUpdate ()
    {
       UpdateLocalPositionDelta();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            GetComponentInParent<StateMachine>().SwitchStates(StateMachine.PlayerStates.IdleGround);
            GameObject.FindObjectOfType<GUIController>().UpdateAltitude(0);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            GameObject.FindObjectOfType<GUIController>().ResetTimer();
        }
    }

    public void Jump(Vector3 Force)
    {
        GetComponent<Rigidbody2D>().AddForce(Force, ForceMode2D.Impulse);
    }

    private void UpdateLocalPositionDelta()
    {
        localPositionDelta = transform.localPosition - lastLocalPosition;
        lastLocalPosition = transform.localPosition;
    }
}
