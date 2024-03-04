using System.Collections.Generic;
using TMPro;
using UnityEngine;
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


    private void Start()
    {
        mainSprite.sprite = Resources.Load<Sprite>($"Texture/Border/{color}/{borderType}");
        textMeshProUGUI.text = GameManager.instance.GetTextData(textId);
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
