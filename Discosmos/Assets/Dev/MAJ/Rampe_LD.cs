using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Rampe_LD : MonoBehaviour
{
    [Header("Curve")]
    [SerializeField] private List<RailPoint> railPoints;
    [SerializeField] private  float nbPoints;
    [SerializeField] private  float distBetweenNodes;
    private List<Vector3> pointsOnCurve =new List<Vector3>(0);
    private List<Vector3> distancedNodes = new List<Vector3>(0);
    [SerializeField] private bool loop;
    [SerializeField] private List<Transform> forms;

    [Header("Ramp")] 
    [SerializeField] private Vector2[] railVertices;
    public MeshFilter meshFilter;
    public int nbRails;
    public float space;

    [Header("Tool")] 
    [SerializeField] private bool generateRail;
    public bool addNewPoint;
    public bool removeLastPoint;
    [SerializeField] private GameObject railPoint;

    [Header("Gameplay")] 
    [SerializeField] private bool playerOnRamp;
    [SerializeField] private AnimationCurve speedBoost;
    private float _progressOnRamp;
    [SerializeField] private float downTimer;
    [SerializeField] private float downDelay;

    [Header("Graphics")] 
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;

    private void Start()
    {
        DrawRailPoints();
        CreateDistancedNodes();
        meshFilter.mesh = GenerateRail();
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;
    }


    void OnEnterRamp(PlayerController playerController)
    {
        
    }
    
    void OnExitRamp(PlayerController playerController)
    {
        // Quand le joueur sort de la rampe, on passe en mode downTime
        // On set le slider du material a 1 automatiquement
    }
    
    void OnRampUp()
    {
        // quand la rampe est de nouveau prenable par le joueur
        // On remet les propriétés du material de base
    }

    private void Update()
    {
        if (playerOnRamp)
        {
            
        }
    }

    private void OnDrawGizmos()
    {
        DrawRailPoints();
        CreateDistancedNodes();
        foreach (Vector3 pos in distancedNodes)
        {
            Gizmos.DrawSphere(pos,0.1f);
        }

        if (generateRail)
        {
            meshFilter.mesh = GenerateRail();
            generateRail = false;
        }
        if (addNewPoint)
        {
            AddNewPoint();
            addNewPoint = false;
        }
        if (removeLastPoint)
        {
            RemoveLastPoint();
            removeLastPoint = false;
        }
    }

    void AddNewPoint()
    {
        GameObject obj = Instantiate(railPoint, transform.position, quaternion.identity,transform);
        obj.name = "Point" + railPoints.Count;
        RailPoint newPoint = new RailPoint();
        newPoint.point = obj.transform;
        newPoint.previousHandle = obj.transform.GetChild(0);
        newPoint.nextHandle = obj.transform.GetChild(1);
        railPoints.Add(newPoint);
    }
    
    void RemoveLastPoint()
    {
        DestroyImmediate(railPoints[railPoints.Count - 1].point.gameObject);
        railPoints.RemoveAt(railPoints.Count - 1);
    }

    Mesh GenerateRail()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>(0);
        List<Color> colors = new List<Color>(0);
        List<int> triangles = new List<int>(0);
        int nbPt = 0;

        for (int x = 0; x < nbRails; x++)
        {
            for (int i = 0; i < distancedNodes.Count; i++)
            {
                Vector3 yAxis = Vector3.up;
                Vector3 xAxis;
                if (i != distancedNodes.Count - 1)
                {
                    xAxis = Quaternion.Euler(0, 90, 0) * (distancedNodes[i + 1] - distancedNodes[i]);
                }
                else if (!loop)
                {
                    xAxis = Quaternion.Euler(0, 90, 0) * (distancedNodes[i] - distancedNodes[i-1]);
                }
                else
                {
                    xAxis = Quaternion.Euler(0, 90, 0) * (distancedNodes[0] - distancedNodes[i]);
                }

                xAxis = xAxis.normalized;

                Color color = Color.Lerp(Color.black, Color.white, i / (float) distancedNodes.Count);
                
                for (int j = 0; j < railVertices.Length; j++)
                {
                    Vector3 pos = distancedNodes[i] + (xAxis * railVertices[j].x) + (-xAxis * (space * (nbRails-1 )/ 2) + xAxis * x * (space)) + yAxis * railVertices[j].y;
                    pos = transform.InverseTransformPoint(pos);
                    vertices.Add(pos);
                    colors.Add(color);
                }
            }

            for (int i = 0; i < distancedNodes.Count-1; i++)
            {
                for (int j = 0; j < railVertices.Length; j++)
                {
                    int a = nbPt + (i * railVertices.Length) + j;
                    int b = nbPt + (i * railVertices.Length) + (j+1) % railVertices.Length;
                    int c = nbPt + ((i+1) * railVertices.Length) + j;
                    int d = nbPt + ((i+1) * railVertices.Length) + (j+1) % railVertices.Length;
                    triangles.AddRange(GetTrianglesForQuad(a,b,c,d));
                }
            }

            if (loop)
            {
                for (int j = 0; j < railVertices.Length; j++)
                {
                    int a = nbPt + (distancedNodes.Count-1) * railVertices.Length + j;
                    int b = nbPt + (distancedNodes.Count-1) * railVertices.Length + (j+1)%railVertices.Length;
                    int c = nbPt + j;
                    int d = nbPt + (j+1)%railVertices.Length; 
                    triangles.AddRange(GetTrianglesForQuad(a,b,c,d));
                }
            }

            nbPt = vertices.Count;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.colors = colors.ToArray();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            
            Debug.DrawRay(transform.position + mesh.vertices[i],mesh.normals[i],Color.green,10);
        }
        return mesh;
    }

    int[] GetTrianglesForQuad(int a,int b,int c,int d)
    {
        List<int> triangles = new List<int>(0);
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        triangles.Add(b);
        triangles.Add(d);
        triangles.Add(c);
        return triangles.ToArray();
    }
    
    void CreateDistancedNodes()
    {
        if (distBetweenNodes <= 0) return;
        distancedNodes.Clear();
        distancedNodes.Add(railPoints[0].point.position);
        float totalDist = 0;
        for (int i = 1; i < pointsOnCurve.Count; i++)
        {
            totalDist += Vector3.Distance(pointsOnCurve[i], pointsOnCurve[i - 1]);
        }
        int numberOfNodes =  Mathf.RoundToInt(totalDist / distBetweenNodes);
        float distNode = totalDist / numberOfNodes;
        numberOfNodes--;

        int index = 1;
        Vector3 current = pointsOnCurve[0];
        for (int i = 0; i < numberOfNodes; i++)
        {
            if (Vector3.SqrMagnitude(pointsOnCurve[index] - current) < distNode * distNode)
            {
                float dist = distNode - Vector3.Distance(pointsOnCurve[index], current);
                index++;
                for (int j = 0; j < 500; j++)
                {
                    if (Vector3.SqrMagnitude(pointsOnCurve[index] - pointsOnCurve[index - 1]) < dist * dist)
                    {
                        dist -= Vector3.Distance(pointsOnCurve[index], pointsOnCurve[index - 1]);
                        index++;
                    }
                    else
                    {
                        Vector3 pos = pointsOnCurve[index-1] + (pointsOnCurve[index] - pointsOnCurve[index-1]).normalized * dist;
                        distancedNodes.Add(pos);
                        current = pos;
                        break;
                    }
                }
            }
            else
            {
                Vector3 pos = current + (pointsOnCurve[index] - current).normalized * distBetweenNodes;
                distancedNodes.Add(pos);
                current = pos;
            }
        }
        if(!loop)distancedNodes.Add(railPoints[railPoints.Count-1].point.position);
    }
    
    private void DrawRailPoints()
    {
        pointsOnCurve.Clear();
        for (int i = 0; i < railPoints.Count-1; i++)
        {
            DrawPoints(railPoints[i].point.position,railPoints[i].nextHandle.position,railPoints[i+1].previousHandle.position,railPoints[i+1].point.position);
        }
        if (loop)
        {
            DrawPoints(railPoints[railPoints.Count-1].point.position,railPoints[railPoints.Count-1].nextHandle.position,railPoints[0].previousHandle.position,railPoints[0].point.position);
            pointsOnCurve.Add(railPoints[0].point.position);
        }
        else
        {
            pointsOnCurve.Add(railPoints[railPoints.Count-1].point.position);   
        }
    }

    void DrawPoints(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
    {
        for (int i = 0; i < nbPoints; i++)
        {
            Vector3 pos = QuadraticLerp(a, b, c, d, (1 / nbPoints) * i);
            pointsOnCurve.Add(pos);
        }
    }

    Vector3 DoubleLerp(Vector3 a,Vector3 b,Vector3 c,float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 abc = Vector3.Lerp(ab, bc, t);
        return abc;
    }

    Vector3 QuadraticLerp(Vector3 a,Vector3 b,Vector3 c,Vector3 d,float t)
    {
        Vector3 abc = DoubleLerp(a, b, c, t);
        Vector3 bcd = DoubleLerp(b, c, d, t);
        Vector3 quadratic = Vector3.Lerp(abc, bcd, t);
        return quadratic;
    }
}

[Serializable]
public class RailPoint
{
    public Transform point;
    public Transform previousHandle;
    public Transform nextHandle;
}
