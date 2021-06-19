using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class LifeGameField : MonoBehaviour
{
    [SerializeField] LifeGameCell m_cellPrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 50;//横の長さ
    [SerializeField] int m_col = 50;//縦の長さ
    [SerializeField] int m_probability = 100;
    private LifeGameCell[,] m_cell;


    void Start()
    {
        m_probability = Mathf.Clamp(m_probability, 0, 100);
        if (m_col < m_row)
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_container.constraintCount = m_row;
        }
        else
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            m_container.constraintCount = m_col;
        }
        m_cell = new LifeGameCell[m_row, m_col];

        //ステージを生成、確率でセルを生きている状態にする
        for (int col = 0; col < m_col; col++)
        {
            for (int row = 0; row < m_row; row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                m_cell[row, col] = cell;
                cell.GetCoordinate(row, col);
                if (m_probability != 0)
                {
                    int random = Random.Range(0, 101);
                    if (random <= m_probability)
                    {
                        m_cell[row, col].LifeState = LifeState.Life;
                    }
                }
            }
        }
    }

    public void SearchAll()
    {
        for (int c = 0; c < m_col; c++)
        {
            for (int r = 0; r < m_row; r++)
            {
                int check = 0;
                check += Search(r, c, -1, 0);//左
                check += Search(r, c, -1, -1);//左上
                check += Search(r, c, 0, -1);//上
                check += Search(r, c, 1, -1);//右上
                check += Search(r, c, 1, 0);//右
                check += Search(r, c, 1, 1);//右下
                check += Search(r, c, 0, 1);//下
                check += Search(r, c, -1, 1);//左下

                if (m_cell[r, c].LifeState == LifeState.Death)
                {
                    if (check == 3)
                    {
                        m_cell[r, c].NextLifeState = NextLifeState.NextLife;
                    }
                    else
                    {
                        m_cell[r, c].NextLifeState = NextLifeState.Maintain;
                    }
                }
                else
                {
                    if (check <= 1 || check >= 4)
                    {
                        m_cell[r, c].NextLifeState = NextLifeState.NextDeath;
                    }
                    else
                    {
                        m_cell[r, c].NextLifeState = NextLifeState.Maintain;
                    }
                }
            }
        }

        foreach (var item in m_cell)
        {
            item.Next();
        }
    }

    public int Search(int row, int col, int moveR, int moveC)
    {
        if (row + moveR < 0 || row + moveR >= m_row || col + moveC < 0 || col + moveC >= m_col)
        {
            return 0;
        }

        if (m_cell[row + moveR, col + moveC].LifeState == LifeState.Life)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScreen");
    }
    float a = 0;
    void Update()
    {

        if (Input.GetMouseButton(0))
        {

            a += Time.deltaTime;
            if (a > 0.1)
            {
                a = 0;
                SearchAll();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            SearchAll();
        }
    }

    public void Restart()
    {
        foreach (var item in m_cell)
        {
            if (m_probability != 0)
            {
                int random = Random.Range(0, 101);
                if (random <= m_probability)
                {
                    item.LifeState = LifeState.Life;
                }
                else
                {
                    item.LifeState = LifeState.Death;
                }
            }
        }
    }
}
