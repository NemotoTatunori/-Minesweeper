using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BingoField : MonoBehaviour
{
    [SerializeField] BingoCell m_cellPrefab;//セルのプレハブ
    [SerializeField] BingoBoardNum m_BoardCellPrefab;//番号板のプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] GridLayoutGroup m_board = null;
    [SerializeField] int m_row = 5;//横の長さ
    [SerializeField] int m_col = 5;//縦の長さ
    [SerializeField] int m_boardRow = 15;//横の長さ
    [SerializeField] int m_boardCol = 5;//縦の長さ
    [SerializeField] Text m_result = null;
    int[,] m_AllNum;//全数字の配列
    BingoCell[,] m_bingoCell;//セルの配列
    BingoBoardNum[,] m_bingoBoardNums;//番号板の配列
    int m_turn = 0;

    void Start()
    {
        m_AllNum = new int[,]{
        { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
        { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45 },
        { 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 },
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
        //ビンゴカードを生成
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

        //ビンゴカードに番号を付与
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

        if (m_boardCol < m_boardRow)
        {
            m_board.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_board.constraintCount = m_boardRow;
        }
        else
        {
            m_board.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_board.constraintCount = m_boardCol;
        }

        m_bingoBoardNums = new BingoBoardNum[m_boardRow, m_boardCol];
        //番号板を生成
        for (int col = 0; col < m_boardCol; col++)
        {
            for (int row = 0; row < m_boardRow; row++)
            {
                var cell = Instantiate(m_BoardCellPrefab);
                var parent = m_board.transform;
                cell.transform.SetParent(parent);
                m_bingoBoardNums[row, col] = cell;
                cell.GetNumder(row + 1 + col * 15);
                if (col == 0)
                {
                    cell.GetColumn("B");
                }
                else if (col == 1)
                {
                    cell.GetColumn("I");
                }
                else if (col == 2)
                {
                    cell.GetColumn("N");
                }
                else if (col == 3)
                {
                    cell.GetColumn("G");
                }
                else
                {
                    cell.GetColumn("O");
                }
            }
        }
    }

    public void Lottery()
    {
        if (m_turn < m_bingoBoardNums.Length)
        {
            while (true)
            {
                //ランダムに値を抽出する
                int col = Random.Range(0, m_boardCol);
                int row = Random.Range(0, m_boardRow);
                if (m_bingoBoardNums[row, col].State == State.Close)
                {
                    m_turn++;
                    m_bingoBoardNums[row, col].State = State.Open;
                    m_result.text = m_turn + "回目：" + m_bingoBoardNums[row, col].m_myColumn + "の" + m_bingoBoardNums[row, col].m_myNumber + "が出た";
                    break;
                }
            }
        }
        else
        {
            Debug.Log("もうない");
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
