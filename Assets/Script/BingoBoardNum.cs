using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoBoardNum : MonoBehaviour
{
    [SerializeField] Image m_color = null;
    [SerializeField] Text m_number = null;
    [SerializeField] State m_state = State.Close;
    public string m_myColumn;
    public int m_myNumber;

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
            m_color.color = Color.red;
        }
        else if (m_state == State.Close)
        {
            m_color.color = Color.white;
        }
    }
    public void GetNumder(int n)
    {
        m_myNumber = n;
        m_number.text = n.ToString();
    }

    public void GetColumn(string c)
    {
        m_myColumn = c;
    }
}
