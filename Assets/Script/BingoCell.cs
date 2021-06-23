using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoCell : MonoBehaviour
{
    [SerializeField] Text m_num = null;
    [SerializeField] State m_state = State.Close;
    [SerializeField] Image m_image = null;
    public int m_row;
    public int m_col;
    public int m_myNum;

    public void GetCoordinate(int r, int c)
    {
        m_row = r;
        m_col = c;
    }

    public State State
    {
        get => m_state;
        set
        {
            m_state = value;
            OnSteteChanged();
        }
    }

    private void OnValidate()
    {
        OnSteteChanged();
    }

    private void OnSteteChanged()
    {
        if (m_state == State.Open)
        {
            m_image.color = Color.gray;
        }
        else if (m_state == State.Close)
        {
            m_image.color = Color.white;
        }
    }

    public void GetMyNum(int n)
    {
        m_myNum = n;
        m_num.text = n.ToString();
        if (m_row == 2 && m_col == 2)
        {
            m_num.text = "F";
            m_myNum = 0;
            m_state = State.Open;
            OnSteteChanged();
        }
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
