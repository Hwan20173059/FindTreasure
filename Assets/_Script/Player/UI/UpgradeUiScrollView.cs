using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUiScrollView : MonoBehaviour
{
    public RectTransform content;
    float minY = 0;
    [SerializeField] float maxY;


    private void LateUpdate()
    {
        float clamp = Mathf.Clamp(content.anchoredPosition.y, minY, maxY);
        content.anchoredPosition = new Vector3(0, clamp, 0);
    }

}
