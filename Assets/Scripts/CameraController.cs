using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject cameraTarget;
    public float positionSmoothTime = 0.2f;
    public float sizeSmoothTime = 0.7f;
    public float cameraTargetVelocityToSizeFactor = .6f;
    public Vector3 cameraOffset = new Vector3(0, 0, -2f);
    private bool cameraLocked;
    private Vector3 posMoveVelocity;
    private float sizeMoveVelocity;
    private float defaultCameraOrthoSize;
    private Camera camera;
    private Rigidbody2D cameraTargetBody;
    private float cameraTargetAveregeVelocityMagnitude;

    // circular buffer for storing velocity magnitudes
    public static readonly int CircularBufferCapacity = 30;
    private Queue<float> cameraTargetVelocityMagnitudes = new Queue<float>(CircularBufferCapacity);

    public void Start()
    {
        camera = Camera.main;
        cameraTargetBody = cameraTarget.GetComponent<Rigidbody2D>();
        defaultCameraOrthoSize = camera.orthographicSize;
        StartCoroutine(LockCamera(.5f));
    }

    public void FixedUpdate()
    {
        if (!cameraLocked)
        {
            Vector3 camPos = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y, 0) + cameraOffset;
            this.transform.position = Vector3.SmoothDamp(transform.position, camPos, ref posMoveVelocity, positionSmoothTime);
            this.GetCameraTargetAveregedVelocity(ref cameraTargetAveregeVelocityMagnitude);
            float desiredOrthographicSize = defaultCameraOrthoSize + cameraTargetAveregeVelocityMagnitude * cameraTargetVelocityToSizeFactor;
            desiredOrthographicSize = Mathf.Clamp(desiredOrthographicSize, defaultCameraOrthoSize, 2.0f);
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, desiredOrthographicSize, ref sizeMoveVelocity, sizeSmoothTime);
        }
    }

    private IEnumerator LockCamera(float duration)
    {
        cameraLocked = true;
        yield return new WaitForSeconds(duration);
        cameraLocked = false;
    }

    private void GetCameraTargetAveregedVelocity(ref float averagedVelocity)
    {
        cameraTargetVelocityMagnitudes.Enqueue(cameraTargetBody.velocity.magnitude);
        float sum = 0f;
        foreach(float item in cameraTargetVelocityMagnitudes)
        {
            sum += item;
        }
        averagedVelocity = sum / cameraTargetVelocityMagnitudes.Count;

        if(cameraTargetVelocityMagnitudes.Count > CircularBufferCapacity)
        {
            cameraTargetVelocityMagnitudes.Dequeue();
        }
    }
}
