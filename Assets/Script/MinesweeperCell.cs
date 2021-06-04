using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperCell : MonoBehaviour
{
    [SerializeField] bool m_bomb = false;
    bool m_Deployment = false;
    bool m_mark = false;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (m_Deployment == false && m_mark == false)
            {

            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            var t = this.GetComponentInChildren<Text>();
            if (this.m_Deployment == false)
            {
                if (this.m_mark == false)
                {
                    this.m_mark = true;
                    t.text = "●";
                }
                else
                {
                    this.m_mark = false;
                    t.text = "";
                }
            }
        }
    }

    public void Bomb(bool morau)
    {    
        m_bomb = morau;
        Debug.Log(m_bomb);
    }
}
