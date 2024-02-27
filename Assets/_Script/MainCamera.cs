using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public float speed;

    public int currentStage = 0;
    public Vector2[] Stage;

    public Vector2 size;

    float height;
    float width;

    private void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for(int i = 0; i < Stage.Length; i++)
            Gizmos.DrawWireCube(Stage[i], size);
    }

    private void LateUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * speed);

        float lx = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + Stage[currentStage].x, lx + Stage[currentStage].x);

        float ly = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + Stage[currentStage].y, ly + Stage[currentStage].y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
}
