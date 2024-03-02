using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MessageUi : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void WriteMessage(UpgradeStats_Base upgradeStats_Base)
    {
        float increse;

        if (upgradeStats_Base.upgradeIncreseType == UpgradeIncreseType.Percent)
            increse = upgradeStats_Base.upgradeAmount / 100;
        else increse = upgradeStats_Base.upgradeAmount;

        string info = $"{upgradeStats_Base.upgradeStateName}\nIncrease : {increse}\n" +
            $"Condition\n";


        for (int i = 0; i < upgradeStats_Base.condition.conditionID.Length; i++)
        {
            info += $"{GetCondition(upgradeStats_Base.condition.conditionID[i])} x {upgradeStats_Base.condition.conditionCount[i]}\n";
        }
            
    }



    string GetCondition(int id)
    {
        string str = "";
        UpgradeStats_Base conditionBase = GameManager.instance.player.GetComponent<PlayerController>().playerStats.upgradeStateDictionary[id];

        str += $"{conditionBase.upgradeStateName}";
        return str;
    }
}
