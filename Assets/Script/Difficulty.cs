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
}
