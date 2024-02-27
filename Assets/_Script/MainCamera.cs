using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public float speed;

    public int currentStage = 0;
    public Vector2[] stageCenter;
    public Vector2[] stageSize;

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

        for(int i = 0; i < stageCenter.Length; i++)
            Gizmos.DrawWireCube(stageCenter[i], stageSize[i]);
    }

    private void LateUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * speed);

        float lx = stageSize[currentStage].x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + stageCenter[currentStage].x, lx + stageCenter[currentStage].x);

        float ly = stageSize[currentStage].y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + stageCenter[currentStage].y, ly + stageCenter[currentStage].y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
}
