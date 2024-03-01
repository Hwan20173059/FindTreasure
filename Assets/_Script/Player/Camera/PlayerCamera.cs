using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform player;
    public float cameraSpeed;

    public int currentStage;

    public GameObject[] stage;

    float width;
    float height;

    private void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
        player = GameManager.instance.player.transform;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime);

        float lx = stage[currentStage].GetComponent<BoxCollider2D>().size.x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + stage[currentStage].GetComponent<Transform>().position.x, lx + stage[currentStage].GetComponent<Transform>().position.x);

        float ly = stage[currentStage].GetComponent<BoxCollider2D>().size.y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + stage[currentStage].GetComponent<Transform>().position.y, ly + stage[currentStage].GetComponent<Transform>().position.y);

        transform.position = new Vector3(clampX, clampY, -10);
    }
}
