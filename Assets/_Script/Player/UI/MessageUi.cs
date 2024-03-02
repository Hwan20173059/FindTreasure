using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MessageUi : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void WriteMessage(UpgradeStats_Base upgradeStats_Base,Vector3 position)
    {
        string increase = "";

        if (upgradeStats_Base.upgradeIncreseType == UpgradeIncreseType.Percent)
            increase = $"{(upgradeStats_Base.upgradeAmount / 100)} %";
        else increase = upgradeStats_Base.upgradeAmount.ToString();

        string title = $"{upgradeStats_Base.upgradeStateName}";

        if(upgradeStats_Base.curCount >= upgradeStats_Base.maxCount)
        {
            title += " [Max]";
        }
        else
        {
            title = title.Replace(" [Max]", "");
        }

        
        string info = $"{title}\n The amount of increase : [ {increase} ]\n" +
            $"\n- Condition -\n";

        if (upgradeStats_Base.condition.conditionID.Length > 0)
        {
            for (int i = 0; i < upgradeStats_Base.condition.conditionID.Length; i++)
            {
                info += $"{upgradeStats_Base.condition.conditionID[i].upgradeStateName}[ {upgradeStats_Base.condition.conditionCount[i]} ]\n";
            }
        }
        else
        {
            info += "No Condition";
        }

       

        text.text = info;
        transform.position = position;
    }

    public  void WriteMessage(UpgradeStats_Base upgradeStats_Base)
    {
        string increase = "";

        if (upgradeStats_Base.upgradeIncreseType == UpgradeIncreseType.Percent)
            increase = $"{(upgradeStats_Base.upgradeAmount / 100)} %";
        else increase = upgradeStats_Base.upgradeAmount.ToString();

        string title = $"{upgradeStats_Base.upgradeStateName}";

        if (upgradeStats_Base.curCount >= upgradeStats_Base.maxCount)
        {
            title += " [Max]";
        }
        else
        {
            title = title.Replace(" [Max]", "");
        }


        string info = $"{title}\n The amount of increase : [ {increase} ]\n" +
            $"\n- Condition -\n";

        if (upgradeStats_Base.condition.conditionID.Length > 0)
        {
            for (int i = 0; i < upgradeStats_Base.condition.conditionID.Length; i++)
            {
                info += $"{upgradeStats_Base.condition.conditionID[i].upgradeStateName}[ {upgradeStats_Base.condition.conditionCount[i]} ]\n";
            }
        }
        else
        {
            info += "No Condition";
        }


        text.text = info;
    }



}
