using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiField : MonoBehaviour
{
    [SerializeField] ReversiPiece m_piecePrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 8;
    [SerializeField] int m_col = 8;
    private ReversiPiece[,] m_piece;
    bool turn = true;
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
                    _piece.State = State.Open;
                    _piece.PieceState = PieceState.White;
                }
                if (row == 3 && col == 4 || row == 4 && col == 3)
                {
                    var _piece = m_piece[row, col];
                    _piece.State = State.Open;
                    _piece.PieceState = PieceState.Black;
                }
            }
        }
    }
    /// <summary>
    /// 置けるかどうかを調べる
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    public void PieceFindOut(int r, int c)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int col = c; col > 0; col--)//左斜め上を調べる
            {
                for (int row = r; row > 0; row--)
                {
                    if (m_piece[row, col].PieceState == PieceState.None)
                    {
                        Debug.Log("左斜め上、判定なし");
                        col = 0;
                        break;
                    }
                    else if (m_piece[row, col].PieceState == PieceState.Black)
                    {
                        col -= 1;
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    void Update()
    {

    }
}
