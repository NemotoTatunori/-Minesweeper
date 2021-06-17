using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoCell : MonoBehaviour
{
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
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
