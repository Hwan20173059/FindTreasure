using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject[] Spots;
    public PlayerCamera playerCamera;

    private void Update()
    {
        SpotSetActiveTrue(playerCamera.currentStage);
    }

    void SpotSetActiveFalse()
    {
        for (int i = 0; i < Spots.Length; i++)
        {
            Spots[i].SetActive(false);
        }
    }

    void SpotSetActiveTrue(int stage)
    {
        SpotSetActiveFalse();
        Spots[stage].SetActive(true);
    }
}
