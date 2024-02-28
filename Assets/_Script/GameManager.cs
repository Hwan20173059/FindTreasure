using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private string textDataFilePath = "Assets/Resources/DataTable/TextData.json";
    public Dictionary<int, string> textDataDictionary { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        // 데이터가 많아지면 매니저를 따로 빼기.
        ReadTextData();
    }

    private void ReadTextData()
    {
        textDataDictionary = new Dictionary<int, string>();
        string jsonText = File.ReadAllText(textDataFilePath);

        TextData[] textDataArray = JsonUtility.FromJson<RootObject>(jsonText).kokr;
        Debug.Log(textDataArray.Length);
        foreach (TextData data in textDataArray)
        {
            textDataDictionary.Add(data.id, data.textKR);
        }
    }

    public string GetTextData(int key)
    {
        return textDataDictionary[key];
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