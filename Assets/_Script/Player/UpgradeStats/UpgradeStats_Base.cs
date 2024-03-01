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
    [Header("Components")]
    PlayerStats playerStats;
    Button button;
    Image image;

    public Condition condition;
    public int id;
    public string upgradeStateName;
    [SerializeField] float upgradeAmount;
    [SerializeField] int maxCount;
    int curCount;
    
    [SerializeField] int cost;

    [Header("State")]
    bool onActive;
    bool isMaxCount;
    [SerializeField] UpgradeStateType upgradeStateType;
    [SerializeField] UpgradeIncreseType upgradeIncreseType;

    public GameObject upgradeCountContain;
    public GameObject upgardeBar;
    GameObject[] upgradeBars;


    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClickEvent);
        upgradeBars = new GameObject[maxCount];
        CreateUpgradeBar();
    }

    private void Start()
    {
        playerStats = GameManager.instance.player.GetComponent<PlayerController>().playerStats;
    }


    void CreateUpgradeBar()
    {
        for (int i = 0; i < maxCount; i++)
        {
           GameObject bar = Instantiate(upgardeBar, upgradeCountContain.transform);
            bar.SetActive(false);
            upgradeBars[i] = bar;
        }
    }

    void UpgradeState() 
    {
        if (!isMaxCount && playerStats.coin >= cost)
        {
            curCount++;
            upgradeBars[curCount - 1].SetActive(true);
            playerStats.coin -= cost;
            cost = cost + cost / 2;

            if (curCount == maxCount)
            {
                isMaxCount = true;
                return;
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
                image.color = Color.white;
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
