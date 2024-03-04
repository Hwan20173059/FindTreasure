using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatsUI : MonoBehaviour
{
    public Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(Close);
    }



    void Close()
    {
        gameObject.SetActive(false);
    }
}
