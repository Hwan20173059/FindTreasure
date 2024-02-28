using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats;
    
    public Text bombCount;
    public Text keyCount;

    public Slider hpBar;

    private void Update()
    {
        bombCount.text = "X " + playerStats.bombAmount.ToString();
        keyCount.text = playerStats.goldenKeyAmount.ToString() + " / 3";

        hpBar.value = playerStats.curHealth / playerStats.health;
    }
}
