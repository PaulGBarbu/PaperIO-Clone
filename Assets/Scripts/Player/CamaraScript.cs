using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CamaraScript : MonoBehaviour
{
    public float camaraHeight = 20f;
    public float smoothSpeed = 0.5f;

    public Player player;
    public Transform rbPlayer;
    
    

    private void FixedUpdate()
    {
        if (player.isAlive)
        {
            Vector3 playerPosition = rbPlayer.position;
            Vector3 targetPosition = new Vector3(playerPosition.x, camaraHeight, playerPosition.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}
