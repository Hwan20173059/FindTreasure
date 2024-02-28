using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 몬스터 스폰과 관리, 상태관리
public class MonstersManager : MonoBehaviour
{
    // 역할1. 스테이지 이름를 받아, MonsterSpawnData에 저장된 정보대로 몬스터를 소환
    // 역할2. ...

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


    private void Start()
    {
        SpawnMonsters("stage_a"); // 스테이지 관리 스테이지에서 이와 같이 스폰하면 될 듯
        //SpawnMonsters("stage1-2");
    }

    // 스테이지 이름을 받아 해당 스테이지에 몬스터를 스폰
    public void SpawnMonsters(string stageName)
    {
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
                    monsterObj.transform.position = new Vector3(monsterObj.transform.position.x, genY, monsterObj.transform.position.z);

                    // MonsterStat MinX, MaxX 설정, bool 관련 설정
                    MonsterStat monsterStat = monsterObj.GetComponent<MonsterStat>();
                    if (monsterStat != null)
                    {
                        monsterStat.MinX = minX;
                        monsterStat.MaxX = maxX;
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

    }
}
