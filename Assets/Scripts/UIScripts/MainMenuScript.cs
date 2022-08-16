using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuScript : MonoBehaviour
{
    public void PlayPlayerVsPlayer()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayPlayerVsBot()
    {
        SceneManager.LoadScene("PlayerVsBot");
    }

}
