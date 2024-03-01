using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Start,
    Pause,
    Ending
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private string textDataFilePath = "Assets/Resources/DataTable/TextData.json";
    public Dictionary<int, string> textDataDictionary { get; private set; }

    public InputActionAsset inputAction;

    [HideInInspector]
    public PlayerState state;
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Init();
    }

    private void Init()
    {
        SetPlayerState(PlayerState.Start);
        // 데이터가 많아지면 매니저를 따로 빼기.
        ReadTextData();
        player = Instantiate(player, Vector3.zero, Quaternion.identity);
    }

    public void SetPlayerState(PlayerState _state)
    {
        if (state != _state)
        {
            state = _state;
            switch (_state)
            {
                case PlayerState.Start:
                    Debug.Log($"GameState:{state}");
                    EnableInputAction();
                    break;
                case PlayerState.Pause:
                    Debug.Log($"GameState:{state}");
                    DisableInputAction();
                    break;
            }
        }
    }

    public void DisableInputAction()
    {
        Debug.Log("DisableInputAction");
        inputAction.Disable();
    }

    public void EnableInputAction()
    {
        Debug.Log("EnableInputAction");
        inputAction.Enable();
    }

    private void ReadTextData()
    {
        textDataDictionary = new Dictionary<int, string>();
        string jsonText = File.ReadAllText(textDataFilePath);

        TextData[] textDataArray = JsonUtility.FromJson<RootObject>(jsonText).kokr;
        //Debug.Log(textDataArray.Length);
        foreach (TextData data in textDataArray)
        {
            textDataDictionary.Add(data.id, data.textKR);
        }
    }

    public string GetTextData(int key)
    {
        return textDataDictionary[key];
    }

    public void SceneChange(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

}


[Serializable]
public class RootObject
{
    public TextData[] kokr;
}

[Serializable]
public class TextData
{
    public int id;
    public string textKR;
}