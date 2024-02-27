using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }

    public GameObject[] monsterPrefabs; // 몬스터 프리팹 배열
    public Transform spawnPoint; // 스폰 위치

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnMonster(int monsterIndex)
    {
        Instantiate(monsterPrefabs[monsterIndex], spawnPoint.position, Quaternion.identity);
    }
}
