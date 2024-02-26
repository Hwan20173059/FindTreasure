using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
    public Animator titleAnim;
    public Animator fadeIn;
    public GameObject player;
    public GameObject backGround;

    public void GameStart()
    {
        backGround.SetActive(false);
        BackGroundAnim();
        Invoke("PlayerAnim", 4.5f);
        Invoke("FadeInAnim", 8f);
        Invoke("SceneChange",9.5f);

    }
   
    public void BackGroundAnim()
    {
        titleAnim.SetTrigger("GameStart");
    }

    public void FadeInAnim()
    {
        fadeIn.SetTrigger("StartGame");
    }

    public void PlayerAnim()
    {
        player.SetActive(true);
    }
    
    public void SceneChange()
    {
        SceneManager.LoadScene("MainScene");
    }
}
