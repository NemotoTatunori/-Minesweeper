using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiField : MonoBehaviour
{
    [SerializeField] ReversiPiece m_piecePrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 8;//横の長さ
    [SerializeField] int m_col = 8;//縦の長さ
    [SerializeField] Text m_WhiteCount = null;
    [SerializeField] Text m_BlackCount = null;
    private ReversiPiece[,] m_piece;//ステージの配列
    bool m_turn = true;
    int m_white = 0;
    int m_black = 0;
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
        m_piece = new ReversiPiece[m_row, m_col];

        //ステージを生成
        for (int col = 0; col < m_piece.GetLength(1); col++)
        {
            for (int row = 0; row < m_piece.GetLength(0); row++)
            {
                var piece = Instantiate(m_piecePrefab);
                var parent = m_container.transform;
                piece.transform.SetParent(parent);
                m_piece[row, col] = piece;
                piece.GetCoordinate(row, col);
                if (row == 3 && col == 3 || row == 4 && col == 4)
                {
                    var _piece = m_piece[row, col];
                    _piece.State = State.StateOpen;
                    _piece.PieceState = PieceState.White;
                }
                if (row == 3 && col == 4 || row == 4 && col == 3)
                {
                    var _piece = m_piece[row, col];
                    _piece.State = State.StateOpen;
                    _piece.PieceState = PieceState.Black;
                }
            }
        }
        Aggregate();
        SearchAll();
    }

    /// <summary>
    /// 手番を変える
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    public void ChangeTurn(int r, int c)
    {
        if (m_turn == true)
        {
            m_piece[r, c].State = State.Open;
            m_piece[r, c].PieceState = PieceState.White;
            m_turn = false;
        }
        else
        {
            m_piece[r, c].State = State.Open;
            m_piece[r, c].PieceState = PieceState.Black;
            m_turn = true;
        }
        SearchAll();
    }

    /// <summary>
    /// 置ける場所を探す
    /// </summary>
    public void SearchAll()
    {
        int AllChangeCount = 0;
        for (int c = 0; c < m_col; c++)
        {
            for (int r = 0; r < m_row; r++)
            {
                int ChangeCount = 0;
                ChangeCount += Search(r, c, -1, 0);//左
                ChangeCount += Search(r, c, -1, -1);//左上
                ChangeCount += Search(r, c, 0, -1);//上
                ChangeCount += Search(r, c, 1, -1);//右上
                ChangeCount += Search(r, c, 1, 0);//右
                ChangeCount += Search(r, c, 1, 1);//右下
                ChangeCount += Search(r, c, 0, 1);//下
                ChangeCount += Search(r, c, -1, 1);//左下
                if (ChangeCount != 0 && m_piece[r, c].State == State.Close)
                {
                    //そのセルの色を変える
                    m_piece[r, c].GetComponent<Image>().color = Color.gray;
                    AllChangeCount++;
                }
                else
                {
                    m_piece[r, c].GetComponent<Image>().color = Color.white;
                }
            }
        }
        if (AllChangeCount == 0)
        {
            if (m_turn == true)
            {
                m_turn = false;
            }
            else
            {
                m_turn = true;
            }
        }
    }

    public int Search(int r, int c, int moveR, int moveC)
    {
        int row = r + moveR;
        int col = c + moveC;
        int checkCount = 0;
        while (row < m_row && col < m_col || row >= 0 && col >= 0)//配列の外側に出たら終わる
        {
            if (row == -1 || col == -1 || row == m_row || col == m_col)
            {
                break;
            }
            if (m_piece[row, col].PieceState == PieceState.None)
            {
                break;
            }
            if (m_turn == true)
            {
                if (m_piece[row, col].PieceState == PieceState.Black)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (m_piece[row, col].PieceState == PieceState.White)
                {
                    if (checkCount != 0)
                    {
                        return 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (m_piece[row, col].PieceState == PieceState.White)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (m_piece[row, col].PieceState == PieceState.Black)
                {
                    if (checkCount != 0)
                    {
                        return 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 調べる全てのパターン
    /// </summary>
    /// <param name="r">横座標</param>
    /// <param name="c">縦座標</param>
    public void PieceChangeAll(int r, int c)
    {
        int ChangeCount = 0;
        ChangeCount += PieceChange(r, c, -1, 0);//左
        ChangeCount += PieceChange(r, c, -1, -1);//左上
        ChangeCount += PieceChange(r, c, 0, -1);//上
        ChangeCount += PieceChange(r, c, 1, -1);//右上
        ChangeCount += PieceChange(r, c, 1, 0);//右
        ChangeCount += PieceChange(r, c, 1, 1);//右下
        ChangeCount += PieceChange(r, c, 0, 1);//下
        ChangeCount += PieceChange(r, c, -1, 1);//左下

        if (ChangeCount == 0)
        {
            Debug.Log("ここには置けない");
        }
        else
        {
            ChangeTurn(r, c);
        }
        Aggregate();
    }

    /// <summary>
    /// 石を置いた所の各方向を調べ、ひっくり返す。
    /// </summary>
    /// <param name="r">横座標</param>
    /// <param name="c">縦座標</param>
    /// <param name="moveR">横方向</param>
    /// <param name="moveC">縦方向</param>
    public int PieceChange(int r, int c, int moveR, int moveC)
    {
        int row = r + moveR;
        int col = c + moveC;
        int checkCount = 0;
        while (row < m_row && col < m_col || row >= 0 && col >= 0)//配列の外側に出たら終わる
        {
            if (row == -1 || col == -1 || row == m_row || col == m_col)
            {
                break;
            }
            if (m_piece[row, col].PieceState == PieceState.None)
            {
                break;
            }
            if (m_turn == true)
            {
                if (m_piece[row, col].PieceState == PieceState.Black)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (m_piece[row, col].PieceState == PieceState.White)
                {
                    if (checkCount != 0)
                    {
                        int ChangeRow = r + moveR;
                        int ChangeCol = c + moveC;
                        while (!(ChangeRow == row && ChangeCol == col))
                        {
                            m_piece[ChangeRow, ChangeCol].PieceState = PieceState.White;
                            ChangeRow += moveR;
                            ChangeCol += moveC;
                        }
                        return 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (m_piece[row, col].PieceState == PieceState.White)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (m_piece[row, col].PieceState == PieceState.Black)
                {
                    if (checkCount != 0)
                    {
                        int ChangeRow = r + moveR;
                        int ChangeCol = c + moveC;
                        while (!(ChangeRow == row && ChangeCol == col))
                        {
                            m_piece[ChangeRow, ChangeCol].PieceState = PieceState.Black;
                            ChangeRow += moveR;
                            ChangeCol += moveC;
                        }
                        return 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return 0;
    }

    void Aggregate()
    {
        m_white = 0;
        m_black = 0;
        foreach (var i in m_piece)
        {
            if (i.PieceState == PieceState.Black)
            {
                m_black++;
            }
            else if (i.PieceState == PieceState.White)
            {
                m_white++;
            }
        }
        m_WhiteCount.text = "白：" + m_white;
        m_BlackCount.text = "黒：" + m_black;
    }

    void GameSet()
    {

    }

    void Update()
    {

    }
}
