using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    float percent;
    public float cameraSpeed;

    private void Update()
    {
 
        transform.position = player.position+offset;
    }
}
