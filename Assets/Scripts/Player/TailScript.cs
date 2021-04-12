using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player), typeof(PlayersArea), typeof(MeshCollider))]
public class TailScript : MonoBehaviour
{
    private const float PointSpacing = 0.1f; 
    
    public Transform head;
    public PlayersArea playersArea;
    public Player player;
    
    private LineRenderer lineRenderer;
    private List<Vector3> linePoints;
    
    private MeshCollider meshCollider;

    private bool isOutside = false; // Always starts inside of own Area

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        lineRenderer = GetComponent<LineRenderer>();

        linePoints = new List<Vector3>();
    }
    
    private void FixedUpdate()
    {
        if (isOutside && IsSpacingEnough() && player.isAlive) {
            AddPoint();
            UpdateTailCollider();
        }
    }

    private void AddPoint()
    {
        Vector3 currentPosition = head.position;

        // Add current Point to List
        linePoints.Add(currentPosition);

        // Increment Max Points of Line renderer
        lineRenderer.positionCount = linePoints.Count;
        
        // Add Point to Line Renderer, to be able to render it 
        lineRenderer.SetPosition(linePoints.Count - 1, currentPosition);
    }

    private bool IsSpacingEnough()
    {
        // There is no other point... Spacing HAS to be enough...
        if (linePoints.Count == 0)
            return true;
        
        Vector3 currentPosition = head.position;
        Vector3 lastPosition = linePoints.Last();
        float distance = Vector3.Distance(currentPosition, lastPosition);

        return distance > PointSpacing;
    }
    
    private void UpdateTailCollider()
    {
        const int reducedBy = 7;
        List<Vector3> colliderVertices = new List<Vector3>();
        var amount = linePoints.Count;

        // A mesh needs a Surface, its created by adding a Vector above the areaVertex
        // But here its possible to just use every 2nd or 3rd or even 4th Vertex instead of all
        for (int i = 0; i < amount - 1; i++)
        {
            if(i % reducedBy != 0) // Half as Many 
                continue;
       
            Vector3 downVert = linePoints.ElementAt(i);
            Vector3 upVert = new Vector3(downVert.x, 1, downVert.z);
            
            colliderVertices.Add(upVert);
            colliderVertices.Add(downVert);
        }

        List<int> triangles = new List<int>();

        // Base Vertex is the Vertex in the Square for reference (Upper Left in this case from which the two Triangles are created)
        int currentBaseVertex = 0;
        
        // Geteil zwei ,weil in colliderVertices UP und Down haben (Man Zeichnet aber die Dreiecke quasi immer von 'Oben Rechts aus' 
        int amountOfSquares = colliderVertices.Count / 2 - 2;
        for (int i = 0; i < amountOfSquares; i++)
        {
            // First Triangle
            triangles.Add(currentBaseVertex);
            triangles.Add(currentBaseVertex + 3);
            triangles.Add(currentBaseVertex + 1);

            // Second Triangle
            triangles.Add(currentBaseVertex);
            triangles.Add(currentBaseVertex + 2);
            triangles.Add(currentBaseVertex + 3);

            currentBaseVertex += 2; // TODO: Perhaps can avoid this and build into amountOfSquares idk tbh maybe not 
        }

        // Create Collider Mesh and set attributes
        Mesh meshForCollider = new Mesh();
        meshForCollider.vertices = colliderVertices.ToArray();
        meshForCollider.triangles = triangles.ToArray();

        //Assign Mesh to MeshCollider
        meshCollider.sharedMesh = meshForCollider;
    }

    // Called by PlayersArea
    public void LeftArea()
    {
        isOutside = true;
        lineRenderer.enabled = true; // Enables Line (Visible again)
     
        AddPoint();
    }

    // Called by PlayersArea
    public void EnteredArea()
    {
        isOutside = false;
        playersArea.AddNewVertices(linePoints);
        
        linePoints.Clear(); // Delete old Line Points
        lineRenderer.positionCount = 0; // Disable Line (No more Visible)
        
        meshCollider.sharedMesh = null;

        //TODO: Update area
    }
    
    // Called on GameOver
    public void RemoveTail()
    {
        linePoints.Clear();
        lineRenderer.enabled = false; // Disable Line (No more Visible)
        meshCollider.sharedMesh = null;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        isOutside = false;
        player.GameOver();
    }
    
}