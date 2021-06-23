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
    BingoCell[,] m_bingoCell;//セルの配列
    BingoBoardNum[,] m_bingoBoardNums;//番号板の配列
    private int m_turn = 0;

    void Start()
    {
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

    int[] colNum = { 0, 1, 2, 3, 4 };
    int emptyCol = 0;//State.Close数が０になった列の数

    /// <summary>
    /// 数字を抽選する
    /// </summary>
    public void Lottery()
    {
        if (m_turn < m_bingoBoardNums.Length)
        {
            int col = Random.Range(0, colNum.Length - emptyCol);//ランダムに列番号を抽出する
            int[] rowNum = new int[15];//State.Closeの数字を保存しておく
            int remainingRow = rowNum.Length;//State.Close数を保存しておく

            for (int rows = 0; rows < rowNum.Length; rows++)//その行のState.Closeを数える
            {
                if (m_bingoBoardNums[rows, colNum[col]].State == State.Close)
                {
                    rowNum[rowNum.Length - 1 - rows] = rows;
                    remainingRow--;
                }
            }

            //State.Closeの数字を保存し終わったらソートする
            for (int f = 1; f < rowNum.Length; f++)//インサートソート
            {
                for (int s = f; s > 0; s--)
                {
                    if (rowNum[s - 1] < rowNum[s])
                    {
                        int seve = rowNum[s - 1];
                        rowNum[s - 1] = rowNum[s];
                        rowNum[s] = seve;
                    }
                }
            }

            int row = Random.Range(0, m_boardRow - remainingRow);//ランダムに行番号を抽出する
            int rowN = rowNum[row];
            m_turn++;
            m_bingoBoardNums[rowN, colNum[col]].State = State.Open;
            m_result.text = m_turn + "回目：" + m_bingoBoardNums[rowN, colNum[col]].m_myColumn + "の" + m_bingoBoardNums[rowN, colNum[col]].m_myNumber + "が出た";
            NumberSearch(colNum[col], m_bingoBoardNums[rowN, colNum[col]].m_myNumber);

            if (remainingRow >= rowNum.Length - 1)//State.Close数が０だったらその行を抽出できないようにする
            {
                int seve = colNum[col];
                colNum[col] = colNum[m_boardCol - 1 - emptyCol];
                colNum[m_boardCol - 1 - emptyCol] = seve;
                emptyCol++;
            }
        }
        else
        {
            Debug.Log("もうない");
        }
    }

    /// <summary>
    /// 抽選番号がビンゴカードにあるか調べる
    /// </summary>
    /// <param name="row">行番号</param>
    /// <param name="number">抽選番号</param>
    void NumberSearch(int row, int number)
    {
        for (int col = 0; col < m_col; col++)
        {
            if (m_bingoCell[row,col].m_myNum == number)
            {
                m_bingoCell[row, col].State = State.Open;
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
