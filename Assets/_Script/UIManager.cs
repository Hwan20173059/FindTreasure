using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public Text lifeCount;
    public Text bombCount;
    public Text keyCount;

    public Slider hpBar;

    private void Update()
    {
        lifeCount.text = $"<i>x {playerStats.lifeCount}</i>";
        bombCount.text = $"x {playerStats.bombAmount}";
        keyCount.text = $"{playerStats.goldenKeyAmount} / 3";

        hpBar.value = playerStats.curHealth / playerStats.health;
    }
}
