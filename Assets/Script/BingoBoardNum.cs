using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoBoardNum : MonoBehaviour
{
    [SerializeField] Image m_color = null;
    [SerializeField] Text m_number = null;
    public int m_myNumber;

    public void GetNumder(int n)
    {
        m_myNumber = n;
        m_number.text = n.ToString();
    }

    public void ColorChange()
    {
        m_color.color = Color.red;
    }
}
