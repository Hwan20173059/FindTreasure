using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public enum UpgradeStatusType
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

    [Header("Data")]
    public Condition condition;
    public List<UpgradeStatus> upgradeStatusList = new List<UpgradeStatus>();
    public List<UpgradeStats_Base> postUpgradeStatus = new List<UpgradeStats_Base>();

    public int id;
    public string upgradeStateName;
    public int maxCount;
    public int curCount;
    [SerializeField] int cost;

    [Header("State")]
    bool onActive;
    bool isMaxCount;

    
    [Header("UI")]
    Button button;
    Image image;
    Color orgColr;
    public List<UpgradeUi_Line> lineList = new List<UpgradeUi_Line>();
    GameObject message;
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
        orgColr = image.color;
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


            foreach(UpgradeStatus upgradeStatus in upgradeStatusList)
            {
                playerStats.UpgradePlayerState(upgradeStatus);
            }

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
        
        onActive = true;

        if(lineList != null)
        {
            foreach (UpgradeUi_Line line in lineList)
            {
                line.Active();
                yield return new WaitForSeconds(.5f);
            }
            image.color = Color.white;
            yield return new WaitForSeconds(.1f);
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

    bool CheckPostStatusActive()
    {
        foreach(UpgradeStats_Base upgradeStats_Base in postUpgradeStatus)
        {
            if (upgradeStats_Base.onActive)
            {
                return false;
            }
        }
        return true;
    }

    #region Mouse interaction


    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CheckPostStatusActive())
            {
                isMaxCount = false;
                if (curCount - 1 >= 0)
                {
                    upgradeBars[curCount - 1].GetComponent<Image>().color = Color.white;
                    curCount--;

                    foreach (UpgradeStatus upgradeStatus in upgradeStatusList)
                    {
                        playerStats.DownGradePlayerState(upgradeStatus);
                    }

                }

                if (curCount == 0)
                {
                    onActive = false;
                    playerStats.upgradeStateDictionary.Remove(id);
                    image.color = orgColr;
                    if (lineList.Count > 0)
                    {
                        foreach (UpgradeUi_Line line in lineList)
                        {
                            line.Reset();
                        }
                    }

                }
            }

            Debug.Log("DownGrade");
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        Vector3 position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, data.position, Camera.main, out position);

        message = UIManager.Instance.pooling.GetPoolItem("Message");
        message.GetComponent<MessageUi>().WriteMessage(this, position);
        message.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData data)
    {
        message.gameObject.SetActive(false);
    }

    #endregion
}

[System.Serializable]
public struct Condition // 
{
    public UpgradeStats_Base[] conditionID;
    public int[] conditionCount;
}

[System.Serializable]
public struct UpgradeStatus
{
    public UpgradeStatusType statusType;
    public UpgradeIncreseType increseType;
    public float amount;
}
