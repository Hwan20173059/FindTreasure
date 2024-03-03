using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class MessageUi : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void WriteMessage(UpgradeStats_Base upgradeStats_Base,Vector3 position)
    {
       
        string title = $"{upgradeStats_Base.upgradeStateName}";

        if(upgradeStats_Base.curCount >= upgradeStats_Base.maxCount)
        {
            title += " [Max]";
        }
        else
        {
            title = title.Replace(" [Max]", "");
        }



        string info = "";

        foreach (UpgradeStatus upgradeStatus in upgradeStats_Base.upgradeStatusList)
        {
            string increase = "";

            if (upgradeStatus.increseType == UpgradeIncreseType.Percent)
                increase = $"{upgradeStatus.amount / 100} %";
            else increase = upgradeStatus.amount.ToString();

            info += $"{upgradeStatus.statusType} + {increase}\n";

        }

        string condition = "";

        if (upgradeStats_Base.condition.conditionID.Length > 0)
        {
            for (int i = 0; i < upgradeStats_Base.condition.conditionID.Length; i++)
            {
                condition += $"{upgradeStats_Base.condition.conditionID[i].upgradeStateName}[ {upgradeStats_Base.condition.conditionCount[i]} ]\n";
            }
        }
        else
        {
            condition += "No Condition";
        }



        text.text = $"{title}\n\n{info}\n\n - Condition -\n{condition}";
        transform.position = position;
    }


    public  void WriteMessage(UpgradeStats_Base upgradeStats_Base)
    {
        string title = $"{upgradeStats_Base.upgradeStateName}";

        if (upgradeStats_Base.curCount >= upgradeStats_Base.maxCount)
        {
            title += " [Max]";
        }
        else
        {
            title = title.Replace(" [Max]", "");
        }



        string info = "";

        foreach (UpgradeStatus upgradeStatus in upgradeStats_Base.upgradeStatusList)
        {
            string increase = "";

            if (upgradeStatus.increseType == UpgradeIncreseType.Percent)
                increase = $"{upgradeStatus.amount / 100} %";
            else increase = upgradeStatus.amount.ToString();

            info += $"{upgradeStatus.statusType} + {increase}\n";

        }

        string condition = "";

        if (upgradeStats_Base.condition.conditionID.Length > 0)
        {
            for (int i = 0; i < upgradeStats_Base.condition.conditionID.Length; i++)
            {
                condition += $"{upgradeStats_Base.condition.conditionID[i].upgradeStateName}[ {upgradeStats_Base.condition.conditionCount[i]} ]\n";
            }
        }
        else
        {
            condition += "No Condition";
        }



        text.text = $"{title}\n\n{info}\n\n - Condition -\n{condition}";
    }



}
