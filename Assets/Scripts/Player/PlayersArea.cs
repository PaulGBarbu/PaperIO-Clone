using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class PlayersArea : MonoBehaviour
{

    private Mesh mesh;
    private MeshCollider meshCollider;
    private List<Vector3> areaVertices;

    public TailScript tail;

    //private bool isInside = true;
    
    private void Awake()
    {
        areaVertices = new List<Vector3>();
        mesh = new Mesh();
        
        meshCollider = GetComponent<MeshCollider>();
        // Needs to be Trigger instead of Collider, since Player has to be able to go through it (This could also be set in Unity in the Inspector)
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    public List<Vector3> GetAreaVertices()
    {
        return areaVertices;
    }
    
    // Called once to init the starting area
    // !! Points are created counter clockwise !!
    public void CreateStartingArea(Vector3 playerPos)
    {
        const int radius = 6;
        const int amountOfEdges = 128;

        for (int i = 0; i < amountOfEdges; i++)
        {
            float angle = i * Mathf.PI * 2f / amountOfEdges;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            areaVertices.Add(newPos + playerPos);
        }
        
        UpdateMesh();
        UpdateCollider();
    }

    public void RemoveArea()
    {
        areaVertices.Clear();
        mesh.Clear();
    }
    
    
    public void AddNewVertices(List<Vector3> newVertices)
    {
        Vector3 firstNewVertex = newVertices[0];
        Vector3 lastNewVertex = newVertices[newVertices.Count - 1];
        
        
        int toRemove1 = FindNearestIndex(firstNewVertex);
        int toRemove2 = FindNearestIndex(lastNewVertex);

        // From the Current Area List, first and Last to be removed
        int firstIndexToRemove;
        int lastIndexToRemove;
        
        
        // used so the first/last Index are in order!
        if (toRemove1 < toRemove2)
        {
            firstIndexToRemove = toRemove1;
            lastIndexToRemove = toRemove2;
        } else {
            firstIndexToRemove = toRemove2;
            lastIndexToRemove = toRemove1;
        }
        
        
        if (NeedsFlip(firstIndexToRemove, lastIndexToRemove, newVertices[0]))
        {
            newVertices.Reverse();
        }
 
        // When adding Vertices there are now two new areas.
        // - The original Area + the new Vertices
        // - The new area defined by the newVertices
        areaVertices = GetBiggerArea(newVertices, firstIndexToRemove, lastIndexToRemove);
        

        UpdateMesh();
        UpdateCollider();
    }

    /**
     * Method is used to determine, if the new Vertices need to be reversed. Cause Player could have made the loop in both directions...
     */
    private bool NeedsFlip (int firstIndexToRemove, int lastIndexToRemove, Vector3 newEdgeStart)
    {
        Vector3 firstEdgeEnd = areaVertices[firstIndexToRemove];
        Vector3 secondEdgeStart = areaVertices[lastIndexToRemove];
        
        // if newEdgeStart is closer to secondEdgeStart, flip the list
        float distance1 = Vector3.Distance(firstEdgeEnd, newEdgeStart);
        float distance2 = Vector3.Distance(secondEdgeStart, newEdgeStart);

        return distance1 > distance2;
    }

    /**
     * Special Case means, the new Area is placed over the start/end of the list.
     * Instead of somewhere in between.
     */
    private List<Vector3> testArea1;
    private List<Vector3> testArea2;

    private List<Vector3> GetBiggerArea(List<Vector3> newVertices, int firstIndexToRemove, int lastIndexToRemove)
    {
        // temp Area 1 = OriginalBorder - Cut + new Area
        // temp Area 2 = Cut + new Area
        List<Vector3> areaVerts1 = new List<Vector3>();
        List<Vector3> areaVerts2 = new List<Vector3>();
        
        areaVerts1.AddRange(areaVertices.GetRange(0, firstIndexToRemove));
        areaVerts1.AddRange(newVertices);
        areaVerts1.AddRange(areaVertices.GetRange(lastIndexToRemove, areaVertices.Count - lastIndexToRemove));
        
        areaVerts2.AddRange(areaVertices.GetRange(firstIndexToRemove, lastIndexToRemove - firstIndexToRemove));
        newVertices.Reverse(); // Needs to be reversed, so the list has all Vertices in the right directions
        areaVerts2.AddRange(newVertices);

        float area1 = Util.CalculateArea(areaVerts1);
        float area2 = Util.CalculateArea(areaVerts2);

        testArea1 = areaVerts1;
        testArea2 = areaVerts2;

        return area1 > area2 ? areaVerts1 : areaVerts2;
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = areaVertices.ToArray();
        mesh.triangles = null; // TODO: triangles

        mesh.RecalculateNormals();
    }

    private void UpdateCollider()
    {
        // Take the Mesh and Copy past it basically 0,5m lower so, -> Create a "Cylinder" Mesh 

        List<Vector3> colliderVertices = new List<Vector3>();
        var amount = areaVertices.Count;
 
        // A mesh needs a Surface, its created by adding a Vector above the areaVertex
        for (int i = 0; i < amount; i++)
        {
            Vector3 downVert = areaVertices.ElementAt(i);
            Vector3 upVert = new Vector3(downVert.x, 1, downVert.z);
            
            colliderVertices.Add(upVert);
            colliderVertices.Add(downVert);
        }

        List<int> triangles = new List<int>();
        
        // Base Vertex is the Vertex in the Square for reference (Upper Left in this case from which the two Triangles are created)
        int currentBaseVertex = 0; 
        for (int i = 0; i < amount - 1; i++)
        {
            // First Triangle
            triangles.Add(currentBaseVertex);
            triangles.Add(currentBaseVertex + 3);
            triangles.Add(currentBaseVertex + 1);
            
            // Second Triangle
            triangles.Add(currentBaseVertex);
            triangles.Add(currentBaseVertex + 2);
            triangles.Add(currentBaseVertex + 3);

            currentBaseVertex += 2;
        }
        
        // Last two Triangles 
        var amountOfCollVertices = colliderVertices.Count;

        // Penultimate Triangle
        triangles.Add(amountOfCollVertices - 2);
        triangles.Add(1);
        triangles.Add(amountOfCollVertices - 1);
        
        // Last Triangle
        triangles.Add(amountOfCollVertices - 2);
        triangles.Add(0);
        triangles.Add(1);
        
        // Create Collider Mesh and set attributes
        Mesh meshForCollider = new Mesh();
        meshForCollider.vertices = colliderVertices.ToArray();
        meshForCollider.triangles = triangles.ToArray();
        
        // Assign Mesh to MeshCollider
        meshCollider.sharedMesh = meshForCollider;
    }
    
    /*
    // Start Drawing Tail
    private void OnTriggerExit(Collider col)
    {
        if (isInside)
        {
            tail.LeftArea();
            isInside = !isInside;
        }
    }
    
    // Stop Drawing Tail
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entered Area!");
        if (!isInside)
        {
            tail.EnteredArea();
            isInside = !isInside;
        }
    }
    */
    
    

    /**
     * @param vert given Vertex
     * Method now searches to the nearest Vertex of the Border to the given Vertex and returns an Integer.
     * Since the AreaBorder are Vertices saved in a List, the index corresponds with the found Vertex
     */
    private int FindNearestIndex(Vector3 vert)
    {
        int vertexIndex = -1;
        float distance = Vector3.Distance(vert, areaVertices[0]);
        
        // Rotate "CounterClockwise" to find nearest AreaVertex
        if (distance > Vector3.Distance(vert, areaVertices[1]))
        {
            for (int i = areaVertices.Count - 1; i >= 0; i--)
            {
                float currentDistance = Vector3.Distance(vert, areaVertices[i]);
                if (distance > currentDistance)
                {
                    distance = currentDistance;
                    vertexIndex = i;
                }
            }
        }
        // Rotate "Clockwise" to find nearest AreaVertex
        else
        {
            for (int i = 0; i < areaVertices.Count; i++)
            {
                float currentDistance = Vector3.Distance(vert, areaVertices[i]);
                if (distance > currentDistance)
                {
                    distance = currentDistance;
                    vertexIndex = i;
                }
            }
        }

        return vertexIndex;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        
        if(areaVertices == null)
            return;

        var offset = new Vector3(0, 1, 0);
        var i = 0;
        float j = 0;
        
        // Area Border
        foreach (var vertex in areaVertices)
        {
            // Draw with Offset
            // i++;
            // offset = new Vector3(0, 1f * (i/100f), 0);
            // Gizmos.DrawSphere(vertex + offset, .1f);
            
            // Make them Thiccer
            Gizmos.DrawSphere(vertex, 0.1f + j);
            j += 0.002f;

            // Draw without Offset
            //Gizmos.DrawSphere(vertex, .2f);
        }
    }

   
}
