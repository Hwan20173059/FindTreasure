using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum UpgradeStateType
{
    AttackDamage,
    AttackSpeed,
    Heath,
}

public enum UpgradeIncreseType
{
    Percent,
    FixedValue
}

public class UpgradeStats_Base : MonoBehaviour
{

    PlayerStats playerStats;
    public Condition condition;
    public int id;
    public string upgradeStateName;
    [SerializeField] UpgradeStateType upgradeStateType;
    [SerializeField] UpgradeIncreseType upgradeIncreseType;
    [SerializeField] float upgradeAmount;

    [SerializeField] int maxCount;
    int curCount;
    bool isMaxCount;
    bool onActive;
    [SerializeField] int cost;

    [Header("Interaction")]
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickEvent);
    }

    private void Start()
    {
        playerStats = GameManager.instance.player.GetComponent<PlayerController>().playerStats;
    }

    void UpgradeState() // Check coin amount in playerState.
    {
        if (!isMaxCount)
        {
            curCount++;
            if (curCount == maxCount)
            {
                isMaxCount = true;
            }
        }
       
    }


    public void OnClickEvent()
    {
        if (CheckCondition())
        {
            if (!onActive)
            {
                playerStats.upgradeStateDictionary.Add(id, this);
                onActive = true;
            }
            Debug.Log(playerStats.upgradeStateDictionary[id].upgradeStateName);
            UpgradeState();
        }
        else
        {
            Debug.Log("You don't have satisfaction this Upgrade Conditions");
        }

    }


    bool CheckCondition()
    {
        if(condition.conditionID == null)
        {
            return true;
        }

        int satisfactionCount = condition.conditionID.Length;
        int curSatisFaction = 0;
        for (int i = 0; i < condition.conditionID.Length; i++)
        {
            if (playerStats.upgradeStateDictionary.ContainsKey(condition.conditionID[i]))
            {
                UpgradeStats_Base curBase = playerStats.upgradeStateDictionary[condition.conditionID[i]];
                if(curBase.curCount == condition.conditionCount[i])
                {
                    curSatisFaction++;
                }
            }
        }

        if(curSatisFaction == satisfactionCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}

[System.Serializable]
public struct Condition // 
{
    public int[] conditionID;
    public int[] conditionCount;
}
