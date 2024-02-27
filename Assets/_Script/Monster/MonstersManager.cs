using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonstersManager : MonoBehaviour
{
    // 몬스터 스폰과 관리, 상태관리

    // 1. 스테이지 번호를 매개변수로 받아, MonsterSpawnData에 저장된 정보대로 몬스터를 소환하는 메서드
    // string monsterType에 적힌 문자열은, 프리팹의 이름과 같다.

    private void Start()
    {
        transform.GetComponent<MonsterSpawnData>().init();
        SpawnMonsters("stage1-1");
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
            // 프리팹 로드 경로 설정
            string prefabPath = $"Prefabs/Monsters/{spawnInfo.monsterType}";
            // 프리팹 로드
            GameObject monsterPrefab = Resources.Load<GameObject>(prefabPath);
            if (monsterPrefab != null)
            {
                // 몬스터 인스턴스 생성 및 스폰 위치 설정
                GameObject monterObj = Instantiate(monsterPrefab, spawnInfo.spawnPosition, Quaternion.identity);

                // Todo. 스폰된 위치 바로 아래의 콜라이더 좌우 확인하고, 이동가능 거리 설정

            }
            else
            {
                Debug.LogError($"Monster prefab not found for type {spawnInfo.monsterType} at path {prefabPath}");
            }
        }
    }



}
