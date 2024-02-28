using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterSpawnData", menuName = "_CustomSO/MonsterSpawnData", order = 0)]
public class MonsterSpawnData : ScriptableObject
{
    public List<StageSpawnInfo> stageSpawnInfos;

    // 스테이지 이름을 통해 해당 스테이지의 스폰 정보를 찾기
    public StageSpawnInfo GetSpawnInfoByStageName(string stageName)
    {
        foreach (var stageSpawnInfo in stageSpawnInfos)
        {
            if (stageSpawnInfo.stageName == stageName)
            {
                return stageSpawnInfo;
            }
        }
        return null;
    }
}

[System.Serializable]
public class StageSpawnInfo
{
    public string stageName;
    public List<SpawnInfo> spawnInfos;
}

[System.Serializable]
public struct SpawnInfo
{
    public string monsterType;
    public bool isStopOnIdle;
    public bool isStopOnTrack;
    public bool isBoss;
    public Vector2 spawnPosition;
}
