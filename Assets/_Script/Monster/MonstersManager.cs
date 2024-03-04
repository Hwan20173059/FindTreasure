using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

// 몬스터 스폰과 관리, 상태관리
public class MonstersManager : MonoBehaviour
{
    // 역할1. 스테이지 이름를 받아, MonsterSpawnData에 저장된 정보대로 몬스터를 소환
    // 역할2. ...

    List<GameObject> SpawnedMonsters = new List<GameObject>();

    // 싱글톤(외부 스크립트에서 접근 용이)
    public static MonstersManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스가 생성되면 파괴
        }
    }

    public MonsterSpawnData monsterSpawnData;

    public PlayerCamera PlayerCamera;
    public int currentStage;
    public GameObject[] stage;

    private void Start()
    {
        SpawnMonsters("Test");
        currentStage = -1;
        //SpawnMonsters("stage_a"); // 스테이지 관리 스크립트에서 이와 같이 스폰하면 될 듯 -> 스테이지 관리 스크립트... 그런 건 없었다.
        ////SpawnMonsters("stage1-2");
    }

    private void LateUpdate()
    {
        try
        {
            if (PlayerCamera.currentStage != currentStage)
            {
                currentStage = PlayerCamera.currentStage;
                try
                {
                    SpawnMonsters($"Stage{currentStage}");
                }
                catch
                {
                    Debug.Log($"Stage{currentStage}에 대한 스폰에 실패했습니다.");
                }
            }
        }
        catch
        {
            Debug.Log("MonsterManager - LateUpdate를 실행 할 수 없습니다.");
        }
    }

    // 스테이지 이름을 받아 해당 스테이지에 몬스터를 스폰
    public void SpawnMonsters(string stageName)
    {
        // 현재 스폰되어 있는 모든 몬스터 파괴
        DestroyAllMonsters();

        var spawnInfo = monsterSpawnData.GetSpawnInfoByStageName(stageName);
        if (spawnInfo != null)
        {
            SpawnMonstersFromList(spawnInfo.spawnInfos);
        }
    }

    private void SpawnMonstersFromList(List<SpawnInfo> spawnData)
    {
        foreach (var spawnInfo in spawnData)
        {
            string prefabPath = $"Prefabs/Monsters/{spawnInfo.monsterType}";
            GameObject monsterPrefab = Resources.Load<GameObject>(prefabPath);
            if (monsterPrefab != null)
            {
                GameObject monsterObj = Instantiate(monsterPrefab, spawnInfo.spawnPosition, Quaternion.identity);
                SpawnedMonsters.Add(monsterObj);

                // 현재 몬스터 Stat의 MinX, MaxX 등을 재설정하기 위해 준비
                MonsterStat monsterStat = monsterObj.GetComponent<MonsterStat>();

                // 스폰 시 Y값 및, X범위를 바로 아래 플랫폼의 모양에 따라 재할당
                if (spawnInfo.autoAdjustPosition)
                {
                    // "Ground"와 "Passthrough" 레이어에만 있는 물체를 감지하기 위한 LayerMask 생성
                    int layerMask = LayerMask.GetMask("Ground", "Passthrough");

                    // ray 쏘기
                    RaycastHit2D hit = Physics2D.Raycast(spawnInfo.spawnPosition, Vector2.down, Mathf.Infinity, layerMask);

                    if (hit.collider != null)
                    {
                        Vector3 monsterPosition = spawnInfo.spawnPosition;

                        // 몬스터의 Y 위치 설정
                        monsterPosition.y = hit.point.y;
                        monsterObj.transform.position = monsterPosition;

                        // 이동 범위 설정
                        float leftBoundary = FindBoundary(monsterObj, Vector2.left, layerMask);
                        float rightBoundary = FindBoundary(monsterObj, Vector2.right, layerMask);

                        BoxCollider2D monsterCollider = monsterObj.GetComponent<BoxCollider2D>();
                        float colliderHalfWidth = monsterCollider != null ? monsterCollider.size.x * monsterObj.transform.localScale.x / 2f : 0;
                        monsterStat.MinX = Mathf.Min(spawnInfo.spawnPosition.x, leftBoundary + colliderHalfWidth);
                        monsterStat.MaxX = Mathf.Max(rightBoundary - colliderHalfWidth, spawnInfo.spawnPosition.x);

                    }
                }
                // 스폰 시 위치 및 X범위를 수동으로 할당
                else
                {
                    // x 이동 범위 수동 지정
                    monsterStat.MinX = spawnInfo.spawnPosition.x - spawnInfo.tempMinX;
                    monsterStat.MaxX = spawnInfo.spawnPosition.x + spawnInfo.tempMaxX;
                }

                monsterStat.IsStopOnIdle = spawnInfo.isStopOnIdle;
                monsterStat.IsStopOnTrack = spawnInfo.isStopOnTrack;

            }
            else
            {
                Debug.LogError($"프리팹({spawnInfo.monsterType})을 경로({prefabPath})에서 찾을 수 없음");
            }
        }
    }

    // 몬스터의 X이동범위 자동으로 할당할 경우 탐색
    float FindBoundary(GameObject monster, Vector2 direction, int layerMask)
    {
        float step = 0.05f; // Raycast를 발사할 간격
        float maxDistance = 20f; // 최대 탐색 거리
        float rayLength = monster.GetComponent<BoxCollider2D>().size.y; // 몬스터 콜라이더의 높이

        // 몬스터 하단에서 시작
        Vector2 basePosition = new Vector2(monster.transform.position.x, monster.transform.position.y);
        float goalPositionX = basePosition.x;

        for (float distance = 0; distance <= maxDistance; distance += step)
        {
            Vector2 origin = basePosition + (direction * distance);
            Vector2 rayStartBelow = origin + new Vector2(0, 0.01f); // 아래쪽 Ray 시작점
            Vector2 rayStartAbove = origin + new Vector2(0, 0.1f); // 위쪽 Ray 시작점
            // 시각화 FOR DEBUG
            //Debug.DrawRay(rayStartBelow, Vector2.down * 0.05f, Color.red, 60f);
            //Debug.DrawRay(rayStartAbove, Vector2.up * rayLength, Color.blue, 60f);

            // 아랫방향으로 짧은 Raycast 발사
            RaycastHit2D hitBelow = Physics2D.Raycast(rayStartBelow, Vector2.down, 0.05f, layerMask);

            // 몬스터 위치보다 약간 위에서 위쪽으로 Raycast 발사
            RaycastHit2D hitAbove = Physics2D.Raycast(rayStartAbove + new Vector2(0, 0.05f), Vector2.up, rayLength, layerMask);

            // 아래에는 플랫폼이 있고, 위에는 플랫폼이 없는 경우
            if (hitBelow.collider != null && !hitAbove.collider)
            {
                // 이동 가능한 경계를 찾음
                goalPositionX = basePosition.x + direction.x * distance;
            }
            else break;
        }

        return goalPositionX;
    }

    private void DestroyAllMonsters()
    {
        // 리스트에 있는 모든 몬스터 오브젝트를 파괴
        foreach (var monster in SpawnedMonsters)
        {
            Destroy(monster);
        }

        // 리스트 비우기
        SpawnedMonsters.Clear();
    }
}
