using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject cameraTarget;
    public Vector3 offset;
    public float lerpSpeed = 3.0f;
    private bool cameraLocked;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LockCamera(.5f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!cameraLocked)
        {
            Vector3 camPos = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y, -10.0f) + offset;
            this.transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime * lerpSpeed);
        }
    }

    private IEnumerator LockCamera(float duration)
    {
        cameraLocked = true;
        yield return new WaitForSeconds(duration);
        cameraLocked = false;
    }
}
