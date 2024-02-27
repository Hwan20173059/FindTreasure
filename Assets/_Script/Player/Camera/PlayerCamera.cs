using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float cameraSpeed;

    public int currentStage;

    public Vector2[] center;
    public Vector2[] size;

    float width;
    float height;

    private void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime);

        float lx = size[currentStage].x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + center[currentStage].x, lx + center[currentStage].x);

        float ly = size[currentStage].y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + center[currentStage].y, ly + center[currentStage].y);

        transform.position = new Vector3(clampX, clampY, -10);
    }
}
