using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoCell : MonoBehaviour
{
    [SerializeField] Text m_num = null;
    public int m_row;
    public int m_col;
    int m_myNum;

    public void GetCoordinate(int r, int c)
    {
        m_row = r;
        m_col = c;
    }

    public void GetMyNum(int n)
    {
        m_myNum = n;
        m_num.text = n.ToString();
        if (m_row == 2 && m_col == 2)
        {
            m_num.text = "F";
        }
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
