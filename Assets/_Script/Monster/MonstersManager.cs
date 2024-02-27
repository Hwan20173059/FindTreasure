using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 몬스터 스폰과 관리, 상태관리
public class MonstersManager : MonoBehaviour
{
    // 역할1. 스테이지 번호를 매개변수로 받아, MonsterSpawnData에 저장된 정보대로 몬스터를 소환하는 메서드
    // string monsterType에 적힌 문자열은, 프리팹의 이름과 같다.
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

        // 초기화
        transform.GetComponent<MonsterSpawnData>().init();
    }


    private void Start()
    {
        transform.GetComponent<MonsterSpawnData>().init();
        SpawnMonsters("stage1-1");
        SpawnMonsters("stage1-2");
    }

    // 스테이지 이름 또는 번호로부터 몬스터의 스폰 위치 정보를 받아온다.
    private List<SpawnInfo> GetMonsterSpawnData(string stageName)
    {
        int stageNum = transform.GetComponent<MonsterSpawnData>().stageToIndex[stageName];
        return GetMonsterSpawnData(stageNum);

    }
    private List<SpawnInfo> GetMonsterSpawnData(int stageNum)
    {
        return transform.GetComponent<MonsterSpawnData>().indexToSpawnInfo[stageNum];
    }

    // 스테이지의 이름 또는 번호를 받아 몬스터를 스폰한다.
    private void SpawnMonsters(string stageName)
    {
        List<SpawnInfo> spawnData = GetMonsterSpawnData(stageName);
        SpawnMonstersFromList(spawnData);
    }
    private void SpawnMonsters(int stageNum)
    {
        List<SpawnInfo> spawnData = GetMonsterSpawnData(stageNum);
        SpawnMonstersFromList(spawnData);
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

                // 광선 쏘기
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

                    // MonsterStat 컴포넌트 찾기 및 MinX, MaxX 설정
                    MonsterStat monsterStat = monsterObj.GetComponent<MonsterStat>();
                    if (monsterStat != null)
                    {
                        monsterStat.MinX = minX;
                        monsterStat.MaxX = maxX;
                    }
                }
            }
            else
            {
                Debug.LogError($"Monster prefab not found for type {spawnInfo.monsterType} at path {prefabPath}");
            }
        }
    }



}
