using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public MainCamera mainCamera;
    public int stage;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.collider.tag == "player")
        {
            mainCamera.currentStage = stage;
        }
    }
}
