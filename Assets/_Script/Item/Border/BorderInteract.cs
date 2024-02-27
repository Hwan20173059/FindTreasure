using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.MaterialProperty;

enum BorderType
{
    Alert,
    Boss,
    Down,
    Left,
    Right,
    Up
}

enum BorderColor
{
    Black,
    Red
}

public class BorderInteract : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private BorderType borderType;
    [SerializeField] private BorderColor color;

    [Header("Text")]
    [SerializeField] private Canvas textCanvas;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    // TODO : 나중에 텍스트 데이터 따로 빼서 아이디만 넣어놓고 호출되도록 변경
    [SerializeField] private int textId;

    [Header("Layer")]
    [SerializeField] private LayerMask layerMask;


    // TODO : 매니저등으로 데이터 나누기
    private Dictionary<int, string> textDic = new Dictionary<int, string>();


    private void Awake()
    {
        textDic.Add(0, "fewsfes545464\n054054\n054054\n김이박최\n054 054\n나중에 텍스트 데이터 따로 빼서\n아이디만 넣어놓고 호출되도록 변경\n0\n450\n4");
        textDic.Add(1, "S키를 눌러 아래로 점프합니다.");

        mainSprite.sprite = Resources.Load<Sprite>($"Texture/Border/{color}/{borderType}");
        textMeshProUGUI.text = textDic[textId];
        textCanvas.gameObject.SetActive(false);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (layerMask.value == (layerMask.value | (1 << collision.gameObject.layer)))
        {
            textCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (layerMask.value == (layerMask.value | (1 << collision.gameObject.layer)))
        {
            textCanvas.gameObject.SetActive(false);
        }
    }
}
