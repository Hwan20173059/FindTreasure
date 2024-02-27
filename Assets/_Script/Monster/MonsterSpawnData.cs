using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SpawnInfo
{
    public string monsterType;
    public bool isBoss;
    public Vector2 spawnPosition;
    //public MonsterStat stats;
}

public class MonsterSpawnData : MonoBehaviour
{
    public Dictionary<string, int> stageToIndex = new Dictionary<string, int>();
    public Dictionary<int, List<SpawnInfo>> indexToSpawnInfo = new Dictionary<int, List<SpawnInfo>>();
    public void init()
    {
        stageToIndex = new Dictionary<string, int>{
            {"stage1-1", 1},
            {"stage1-2", 2},
            {"stage1-3", 3},
            {"stage1-4", 4},
            {"stage1-5", 5},
        };

        indexToSpawnInfo = new Dictionary<int, List<SpawnInfo>>
        {
            {
                1, new List<SpawnInfo>
                {
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(0, 0),
                    },
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(1, 1),
                    },
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(-1, -1),
                    },
                    // 다른 몬스터 정보 추가
                }
            },
            {
                2, new List<SpawnInfo>
                {
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(0, 0),
                    },
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(-1, 1),
                    },
                    new SpawnInfo
                    {
                        monsterType = "MonsterA",
                        isBoss = false,
                        spawnPosition = new Vector2(-1, 1),
                    },
                    // 다른 몬스터 정보 추가
                }
            },
            // 다른 스테이지 정보 추가
        };
    }
}
