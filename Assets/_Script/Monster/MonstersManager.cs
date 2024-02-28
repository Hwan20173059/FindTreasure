using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

                // "Ground"와 "Passthrough" 레이어에만 있는 물체를 감지하기 위한 LayerMask 생성
                int layerMask = LayerMask.GetMask("Ground", "Passthrough");

                // ray 쏘기
                RaycastHit2D hit = Physics2D.Raycast(spawnInfo.spawnPosition, Vector2.down, 10f, layerMask);
                if (hit.collider != null)
                {
                    // 하단 콜라이더의 경계를 기반으로 MinX와 MaxX 계산
                    BoxCollider2D monsterCollider = monsterObj.GetComponent<BoxCollider2D>();
                    float colliderHalfWidth = monsterCollider != null ? monsterCollider.size.x * monsterObj.transform.localScale.x / 2f : 0;
                    float minX = hit.collider.bounds.min.x + colliderHalfWidth;
                    float maxX = hit.collider.bounds.max.x - colliderHalfWidth;

                    float genY = hit.collider.bounds.max.y;
                    // 해결 될 때 까지 임시조치
                    //monsterObj.transform.position = new Vector3(monsterObj.transform.position.x, genY, monsterObj.transform.position.z);

                    // MonsterStat MinX, MaxX 설정, bool 관련 설정
                    MonsterStat monsterStat = monsterObj.GetComponent<MonsterStat>();
                    if (monsterStat != null)
                    {
                        //monsterStat.MinX = minX; // 해결 될 때 까지 임시조치
                        //monsterStat.MaxX = maxX;

                        // 임시조치
                        monsterStat.MinX = spawnInfo.spawnPosition.x - spawnInfo.tempMinX;
                        monsterStat.MaxX = spawnInfo.spawnPosition.x + spawnInfo.tempMaxX;

                        monsterStat.IsStopOnIdle = spawnInfo.isStopOnIdle;
                        monsterStat.IsStopOnTrack = spawnInfo.isStopOnTrack;
                    }
                }
            }
            else
            {
                Debug.LogError($"NotFound : MonsterPrefab {spawnInfo.monsterType} | path {prefabPath}");
            }
        }
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
