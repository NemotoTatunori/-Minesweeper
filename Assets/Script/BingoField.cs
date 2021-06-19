using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BingoField : MonoBehaviour
{
    [SerializeField] BingoCell m_cellPrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 5;//横の長さ
    [SerializeField] int m_col = 5;//縦の長さ
    int[,] m_AllNum = new int[4, 15];
    int[] m_B = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    int[] m_I = { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
    int[] m_N = { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 };
    int[] m_G = { 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 };
    int[] m_O = { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };
    BingoCell[,] m_bingoCell;


    void Start()
    {
        m_AllNum = new int[,]{
        { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, },
        { 16,17,18,19,20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,},
        { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 , 41, 42, 43, 44, 45,},
        { 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60  },
        { 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 }
        };

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
        m_bingoCell = new BingoCell[m_row, m_col];
        //ステージを生成
        for (int col = 0; col < m_col; col++)
        {
            for (int row = 0; row < m_row; row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                m_bingoCell[row, col] = cell;
                cell.GetCoordinate(row, col);
            }
        }

        //番号を付与
        for (int row = 0; row < m_row; row++)
        {
            int[] nums = new int[5];//抽選結果を仮に入れておく配列
            for (int col = 0; col < m_col; col++)
            {
                bool Substitution = false;
                int num = Random.Range(1 + (row * 15), 16 + (row * 15));//数字を抽選する
                foreach (var i in nums)//抽選した数字がすでに出ているか調べる
                {
                    if (i == num)
                    {
                        col--;
                        Substitution = false;
                        break;
                    }
                    else
                    {
                        Substitution = true;
                    }
                }
                if (Substitution == true)//抽選した数字が出ていなかったら抽選結果に加える
                {
                    nums[col] = num;
                }                
            }
            //抽選が終わったらソートする
            for (int f = 1; f < nums.Length; f++)//インサートソート
            {
                for (int s = f; s > 0; s--)
                {
                    if (nums[s - 1] > nums[s])
                    {
                        int seve = nums[s - 1];
                        nums[s - 1] = nums[s];
                        nums[s] = seve;
                    }
                }
            }
            //ソートが終わったらそれぞれのセルに抽選結果を送る
            for (int col = 0; col < nums.Length; col++)
            {
                m_bingoCell[row, col].GetMyNum(nums[col]);
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
