using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum UpgradeStateType
{
    AttackDamage,
    AttackSpeed,
    MoveSpeed,
    Heath,
}

public enum UpgradeIncreseType
{
    Percent,
    FixedValue
}

public class UpgradeStats_Base : MousePointerEntity
{
    [Header("Components")]
    PlayerStats playerStats;
    Button button;
    Image image;

    public Condition condition;
    public int id;
    public string upgradeStateName;
    public float upgradeAmount;
    public int maxCount;
    public int curCount;
    
    [SerializeField] int cost;

    [Header("State")]
    bool onActive;
    bool isMaxCount;
    public UpgradeStateType upgradeStateType;
    public UpgradeIncreseType upgradeIncreseType;

    public GameObject upgradeCountContain;
    public GameObject upgardeBar;
    GameObject[] upgradeBars;

    [Header("UI")]
    public List<UpgradeUi_Line> lineList = new List<UpgradeUi_Line>();
    GameObject message;


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
           upgradeBars[i] = bar;
        }
    }

    void UpgradeState() 
    {
        if (!isMaxCount && playerStats.coin >= cost)
        {
            curCount++;
            upgradeBars[curCount - 1].GetComponent<Image>().color = Color.red;
            playerStats.coin -= cost;
            cost = cost + cost / 2;
            playerStats.UpgradePlayerState(this);

            if (curCount == maxCount)
            {
                isMaxCount = true;
                return;
            }

        }
       
    }

    IEnumerator ActiveAndUpgrade()
    {
        playerStats.upgradeStateDictionary.Add(id, this);
        image.color = Color.white;
        onActive = true;

        foreach (UpgradeUi_Line line in lineList)
        {
            line.Active();
            yield return new WaitForSeconds(.5f);
        }

        UpgradeState();
    }


    public void OnClickEvent()
    {
        if (CheckCondition())
        {
            if (!onActive)
            {
                StartCoroutine(ActiveAndUpgrade());
            }
            else
            {
                UpgradeState();
            }

            message.GetComponent<MessageUi>().WriteMessage(this);
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
            if (playerStats.upgradeStateDictionary.ContainsKey(condition.conditionID[i].id))
            {
                UpgradeStats_Base curBase = playerStats.upgradeStateDictionary[condition.conditionID[i].id];
                if(curBase.curCount >= condition.conditionCount[i])
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


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("DownGrade");
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Enter");
        Vector3 position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, data.position, Camera.main, out position);

        message = UIManager.Instance.pooling.GetPoolItem("Message");
        message.GetComponent<MessageUi>().WriteMessage(this, position);
        message.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData data)
    {
        Debug.Log("Exit");
        message.gameObject.SetActive(false);
    }

}

[System.Serializable]
public struct Condition // 
{
    public UpgradeStats_Base[] conditionID;
    public int[] conditionCount;
}
