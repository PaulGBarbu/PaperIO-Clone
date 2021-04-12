using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TryAndError : MonoBehaviour
{
    private List<Vector3> vertices;

    void Awake()
    {
        vertices = new List<Vector3>();
        Vector3 myPoint = new Vector3(-25, 3, 14);
        CreateCircleVerticesAroundPoint(myPoint, 5, 16);
    }

    private void CreateCircleVerticesAroundPoint(Vector3 player, float radius, int points)
    {
        for (int i = 0; i < 16; i++)
        {
            float angle = i * Mathf.PI * 2f / 16;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            vertices.Add(newPos + player);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 vertex in vertices)
        {
            Gizmos.DrawSphere(vertex, .1f);
        }
    }
}