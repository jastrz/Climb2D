using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public float trajectoryTimeStep = 0.02f;
    public int trajectoryProjectilesCount = 60;

    [Range(0, 1)]
    public float percentHead = 0.4f;

    private LineRenderer lineRenderer;
    private int lengthOfLineRenderer = 10;


    private void Awake()
    {
        this.SetLineRenderer();
        lineRenderer.enabled = false;
    }

    private Vector3 CalculatePosition(Vector3 initialPosition, Vector3 initialVelocity, float elapsedTime)
    {
        var position = Physics.gravity * elapsedTime * elapsedTime * 0.5f + initialVelocity * elapsedTime + initialPosition;
        return position;
    }

    public void DrawTrajectory(Vector3 initialPosition, Vector3 initialVelocity)
    {
        lineRenderer.enabled = true;

        var points = new Vector3[trajectoryProjectilesCount];
        for (int i = 0; i < trajectoryProjectilesCount; i++)
        {
            points[i] = CalculatePosition(initialPosition, initialVelocity, i * trajectoryTimeStep);
        }
        SetArrow(ref points);
        lineRenderer.SetPositions(points);
    }

    private void SetLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = .1f;
        lineRenderer.positionCount = trajectoryProjectilesCount;
    }

    public void DisableTrajectory()
    {
        lineRenderer.enabled = false;
    }

    private void SetArrow(ref Vector3[] points)
    {
        lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, 0.4f),
            new Keyframe(0.999f - percentHead, 0.4f),
            new Keyframe(1 - percentHead, 1f),
            new Keyframe(1, 0f));

        Vector3 arrowOrigin = points[trajectoryProjectilesCount - 4];
        Vector3 arrowTarget = points[trajectoryProjectilesCount - 1];

        points[trajectoryProjectilesCount - 3] = Vector3.Lerp(arrowOrigin, arrowTarget, 0.999f - percentHead);
        points[trajectoryProjectilesCount - 2] = Vector3.Lerp(arrowOrigin, arrowTarget, 1 - percentHead);

    }
}
