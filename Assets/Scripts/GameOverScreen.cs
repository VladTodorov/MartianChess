using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI winnerText;

    public void Setup(int winner)
    {
        gameObject.SetActive(true);
        winnerText.SetText("McLovin");
    }
    
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
