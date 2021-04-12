using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayersArea), typeof(MeshRenderer), typeof(BoxCollider))]
public class Player : MonoBehaviour
{
    private const int SpeedConst = 0;
    
    [Header("Scripts")]
    public TailScript tail;
    public PlayersArea area;
    
    [Header("Params")]
    private float speed = SpeedConst;
    public float turningSpeed = 10f;

    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Vector3 currentPosition;

    public bool isAlive;
    private bool isInside; // might be useless afterall idk, the extra variable...

    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        //Fetch the Rigidbody component you attach from your GameObject
        rb = GetComponent<Rigidbody>();

        isInside = true;
    }

    private void Start()
    {
        rb.position = GetNewSpawnPosition();
        // Create Start Territory around player
        area.CreateStartingArea(rb.position);
        isAlive = true;
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            rb.velocity = transform.forward * speed;
            currentPosition = rb.position;
            //turn();
            turnWASD();

            checkEnterAndExit();
        }
    }

    private void checkEnterAndExit()
    {

        // Leaving Area
        if (isInside && !checkIfInArea())
        {
            isInside = false;
            tail.LeftArea();
        }

        // Entering Area
        if (!isInside && checkIfInArea())
        {
            isInside = true;
            tail.EnteredArea();
        }
    }
    
    // My Public Methods
    private bool checkIfInArea()
    {
        List<Vector3> areaVertices = area.GetAreaVertices();
        return Util.IsPointInPolygon(currentPosition, areaVertices);
    }

    public void GameOver()
    {
        isAlive = false;
        RemovePlayer();
        tail.RemoveTail();
        area.RemoveArea();
        
        // Wait a few seconds or sth... and then Respawn
        Respawn();
    }

    private void RemovePlayer()
    {
        speed = SpeedConst;
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
    
    private void Respawn()
    {
        
        // Clean field
        area.RemoveArea();
        
        // Randomize new Position -> Check if in Area of other player -> If not -> TP player there and create new Area
        Vector3 newStart = GetNewSpawnPosition();
        
        area.CreateStartingArea(newStart);
        
        meshRenderer.enabled = true;
        boxCollider.enabled = true;

        rb.position = newStart;
        isAlive = true;
        isInside = true;
    }

    private Vector3 GetNewSpawnPosition()
    {
        Vector3 newPos;
        const int radius = 43; // A bit smaller than the Platform itself, cause I dont want to deal with the start area being outside of the platform
        bool isOnEmptyField;
        
        do
        {
            // Get Random Angle -> Random point somewhere on a Circle
            float angle = Mathf.PI * 2f * Random.value;
            newPos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            // Move position along the angle, to not always start on the edge of the circle
            newPos *= Random.value;

            if (true) // TODO: Check if Spawn Position in another area or not ( Probably a method for GameManager...)
                isOnEmptyField = true;
            
        } while (!isOnEmptyField);
        
        //return newPos;
        return new Vector3(0, 0, 0);
    }
    
    // My Private Methods
    // TODO: Movment like in Paper.IO with WASD
    private void turn()
    {
        // Turn left
        if (Input.GetKey("a"))
        {
            transform.Rotate(0f,-turningSpeed,0f);
        }
        
        // Turn right
        if (Input.GetKey("d"))
        {
            transform.Rotate(0f,turningSpeed,0f);
        }
    }

    private void turnWASD()
    {
        
        // y =   0: nach Oben
        // y =  90: nach rechts
        // y = -90: nach links
        // y =-180: nach unten

        float angle = transform.rotation.y;
        
        
        // Turn left
        if (Input.GetKey("w"))
        {

            if (angle == 0)
            {
                return;
            }
            
            if (angle < 0)
            {
                transform.Rotate(0f,turningSpeed,0f);
            }
            
            // If facing right, turn left
            if(angle > 0)
            {
                transform.Rotate(0f,-turningSpeed,0f);
            }
        }
        
        // Turn right
        if (Input.GetKey("s"))
        {
            transform.Rotate(0f,turningSpeed,0f);
        }
    }
}
