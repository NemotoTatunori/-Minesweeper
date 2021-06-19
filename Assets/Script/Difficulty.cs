using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Difficulty : MonoBehaviour
{
    
    public void Minesweepe()
    {
        SceneManager.LoadScene("Minesweeper");
    }

    public void Reversi()
    {
        SceneManager.LoadScene("Reversi");
    }

    public void LifeGame()
    {
        SceneManager.LoadScene("LifeGame");
    }

    public void Bingo()
    {
        SceneManager.LoadScene("Bingo");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
