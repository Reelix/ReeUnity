using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GizmoDrawer
    {
        public static void DrawCircle(Color color, Transform transform, float radius)
        {
            Gizmos.color = color;
            Vector3 point = transform.TransformPoint(Vector3.zero);
            Matrix4x4 mtx = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(point, Quaternion.identity, new Vector3(1f, 0f, 1f));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = mtx;
        }
    }
}
