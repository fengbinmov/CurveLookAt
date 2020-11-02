using MOV.Curve;
using UnityEngine;

public class CurvePath : MonoBehaviour
{
    [Range(1, 15)]
    public int smoothLevel = 5;
    public Vector3[] points = new Vector3[] { };
    public float length;
    private Vector3[] smoothpoints = new Vector3[] { };
    public int smoothCount { get { return smoothpoints.Length; } }

    public Vector3 GetPoint(int i)
    {
        return transform.TransformPoint(points[i]);
    }
    private void OnEnable()
    {
        OnValidate();
    }
    public Vector3 Evaluate(float process) {

        if (process < 0)
        {
            process = length - Mathf.Abs(process) % length;
        }
        else
        {
            process = process % length;
        }

        return GetPoint(process);
    }
    public void OnValidate()
    {
        smoothpoints = MCurve.GetSmoothPath(points, smoothLevel);
        length = GetLength();
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < smoothpoints.Length - 1; i++)
        {
            Gizmos.DrawLine(smoothpoints[i], smoothpoints[i + 1]);
        }
    }

    public Vector3 GetSmoothPoint(int i)
    {
        return transform.TransformPoint(smoothpoints[i]);
    }
    float GetLength()
    {
        float length = 0;

        for (int i = 0; i < smoothpoints.Length - 1; i++)
        {
            length += Vector3.Distance(GetSmoothPoint(i), GetSmoothPoint(i + 1));
        }
        return length;
    }

    Vector3 GetPoint(float lenT)
    {
        int a = 0, b = 0;
        float length = 0, length2 = 0;
        for (int i = 0; i < smoothpoints.Length - 1; i++)
        {
            length += Vector3.Distance(GetSmoothPoint((i + smoothpoints.Length - 1) % smoothpoints.Length), GetSmoothPoint(i));
            length2 = length + Vector3.Distance(GetSmoothPoint(i), GetSmoothPoint(i + 1));

            if (lenT >= length && lenT < length2)
            {
                a = i;
                b = i + 1;
                break;
            }
        }

        return Vector3.Lerp(GetSmoothPoint(a), GetSmoothPoint(b), (lenT - length) / (length2 - length));
    }
}
