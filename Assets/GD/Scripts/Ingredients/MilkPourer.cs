


using UnityEngine;

public class MilkPouring : MonoBehaviour
{
    [Header("References")]
    public Transform spoutPoint;
    public LineRenderer milkStream;

    [Header("Pour Settings")]
    public float pourAngle = 50f;
    public float pourLength = 0.5f;
    public int segments = 20;

    [Header("Milk Thickness")]
    public float startWidth = 0.045f;
    public float middleWidth = 0.035f;
    public float endWidth = 0.020f;

    void Start()
    {
        milkStream.positionCount = segments;
        milkStream.enabled = false;
        milkStream.useWorldSpace = true;

        milkStream.numCapVertices = 12;
        milkStream.numCornerVertices = 12;

        milkStream.widthMultiplier = 1f;

        milkStream.widthCurve = new AnimationCurve(
            new Keyframe(0f, startWidth),
            new Keyframe(0.5f, middleWidth),
            new Keyframe(1f, endWidth)
        );
    }

    void Update()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);

        if (angle > pourAngle)
        {
            milkStream.enabled = true;

            Vector3 start = spoutPoint.position;

            Vector3 end = start +
                spoutPoint.forward * pourLength +
                Vector3.down * pourLength;

            Vector3 middle = (start + end) / 2f
                + Vector3.down * 0.12f;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);

                Vector3 point =
                    Mathf.Pow(1 - t, 2) * start +
                    2 * (1 - t) * t * middle +
                    Mathf.Pow(t, 2) * end;

                milkStream.SetPosition(i, point);
            }
        }
        else
        {
            milkStream.enabled = false;
        }
    }
}
