using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Pooling pooling;
    PlayerStats playerStats;

    public Text lifeCount;
    public Text coinCount;
    public Text bombCount;
    public Text keyCount;

    public Slider hpBar;

    public GameObject upgradeUiCanvas;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        playerStats = GameManager.instance.player.GetComponent<PlayerController>().playerStats;
        pooling = GetComponent<Pooling>();
        pooling.CreatePool(transform.Find("PoolingBox"));
        upgradeUiCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UpgradeUICanvas"),transform);
        upgradeUiCanvas.SetActive(false);
    }

    private void Update()
    {
        lifeCount.text = $"<i>x {playerStats.lifeCount}</i>";
        coinCount.text = $"x {playerStats.coin}";
        bombCount.text = $"x {playerStats.bombAmount}";
        keyCount.text = $"{playerStats.goldenKeyAmount} / 3";


        hpBar.value = playerStats.curHealth / playerStats.health;
    }
}
