using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurvePath))]
class CurvePathEditor : Editor
{
    private CurvePath curvePath;
    private void OnSceneGUI()
    {
        if (curvePath == null) curvePath = target as CurvePath;

        for (int i = 0; i < curvePath.points.Length - 1; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(curvePath.GetPoint(i), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curvePath, "Change Look At Target Position");
                curvePath.points[i] = curvePath.transform.InverseTransformPoint(newTargetPosition);

                if (i == 0)
                    curvePath.points[curvePath.points.Length - 1] = curvePath.points[i];
                curvePath.OnValidate();
            }
        }
    }
}