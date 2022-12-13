using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewGenerator : MonoBehaviour
{
    public int precision;
    public float lenght;
    public MeshFilter meshFilter;
    public Mesh mesh;


    private void Awake()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        List<Vector3> vertices = new List<Vector3>(0);
        vertices.Add(Vector3.zero);
        List<int> triangles = new List<int>(0);
        for (int i = 0; i < precision; i++)
        {
            Ray ray = new Ray(transform.position, Quaternion.Euler(0, 360 / precision * i, 0) * Vector3.left);
            if (Physics.Raycast(ray, out RaycastHit hit, lenght))
            {
                Debug.DrawRay(ray.origin,ray.direction*hit.distance);
                vertices.Add(transform.InverseTransformPoint(hit.point));
            }
            else
            {
                Debug.DrawRay(ray.origin,ray.direction*lenght);
                vertices.Add(Vector3.zero+ray.direction*lenght);
            }
        }

        for (int i = 1; i < vertices.Count-1; i++)
        {
            triangles.AddRange(new []{0,i,i + 1});
        }
        triangles.AddRange(new []{0,vertices.Count-1,1});

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }
}
