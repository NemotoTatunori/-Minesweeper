using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperField : MonoBehaviour
{
    [SerializeField] MinesweepeCell m_cellPrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 3;
    [SerializeField] int m_col = 3;
    [SerializeField] int m_bomb = 5;
    int m_flag = 0;//旗の経っている数
    int m_bombFlag = 0;//爆弾セルに旗が乗っている数
    int m_bombOtherThan;//爆弾以外のセルが展開された数
    private MinesweepeCell[,] _cells;
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
        _cells = new MinesweepeCell[m_row, m_col];

        //セルを生成
        for (int col = 0; col < _cells.GetLength(1); col++)
        {
            for (int row = 0; row < _cells.GetLength(0); row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                _cells[row, col] = cell;
                cell.GetCoordinate(row, col);
            }
        }

        //爆弾を付与
        for (var i = 0; i < m_bomb; i++)
        {
            if (m_row * m_col == i)//もし爆弾数がマス数に達したら爆弾付与をやめる
            {
                break;
            }
            var r = Random.Range(0, m_row);//横の抽選
            var c = Random.Range(0, m_col);//縦の抽選
            var cells = _cells[r, c];
            if (cells.CellState != CellState.Mine)//もし選ばれたセルが爆弾持ちでなかったら
            {
                cells.CellState = CellState.Mine;//爆弾を付与
            }
            else
            {
                i--;//そうでなかったら抽選をやり直す
            }
        }

        //数字振り1
        /*
        for (int col = 0; col < _cells.GetLength(1); col++)
        {
            for (int row = 0; row < _cells.GetLength(0); row++)
            {
                if (_cells[row, col].CellState == CellState.Mine)
                {
                    continue;
                }
                else
                {
                    int b = 0;
                    for (int c = -1; c < 2; c++)
                    {
                        for (int r = -1; r < 2; r++)
                        {
                            if (row + r == -1 || col + c == -1 || row + r == _cells.GetLength(0) || col + c == _cells.GetLength(1))
                            {
                                continue;
                            }
                            else if (_cells[row + r, col + c].CellState == CellState.Mine)
                            {
                                b++;
                            }
                        }
                    }
                    _cells[row, col].CellState = (CellState)b;
                }
            }
        }
        */

        //数字振り2
        for (int col = 0; col < _cells.GetLength(1); col++)
        {
            for (int row = 0; row < _cells.GetLength(0); row++)
            {
                if (_cells[row, col].CellState != CellState.Mine)
                {
                    continue;
                }
                else
                {
                    for (int c = -1; c < 2; c++)
                    {
                        for (int r = -1; r < 2; r++)
                        {
                            if (row + r == -1 || col + c == -1 || row + r == _cells.GetLength(0) || col + c == _cells.GetLength(1))
                            {
                                continue;
                            }
                            else if (_cells[row + r, col + c].CellState != CellState.Mine)
                            {
                                _cells[row + r, col + c].CellState += 1;
                            }
                        }
                    }
                }
            }
        }

        /*
        //数字振り3        
        for (int r = 0; r < m_row; r++)
        {
            for (int c = 0; c < m_col; c++)
            {
                var cell = _cells[r, c];
                if (cell.CellState == CellState.Mine)
                {
                    continue;
                }
                var count = GetMineCount(r, c);
                cell.CellState = (CellState)count;
            }
        } */
    }

    /// <summary>
    /// 開いたセルの周りに空白があるか探す
    /// </summary>
    /// <param name="ro">横座標</param>
    /// <param name="co">縦座標</param>
    public void CellCoordinate(int ro, int co)
    {
        for (int c = -1; c < 2; c++)
        {
            for (int r = -1; r < 2; r++)
            {
                if (ro + r == -1 || co + c == -1 || ro + r == _cells.GetLength(0) || co + c == _cells.GetLength(1))
                {
                    continue;
                }
                else if (r == 0 && c == 0)
                {
                    continue;
                }
                else if (_cells[ro + r, co + c].Status != Status.Open)
                {
                    if (_cells[ro + r, co + c].CellState == CellState.None)
                    {
                        if (_cells[ro + r, co + c].Status == Status.Flag)//旗だったら展開しない
                        {
                            continue;
                        }
                        _cells[ro + r, co + c].Status = (Status)3;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 空白かつ旗でないなら展開する
    /// </summary>
    /// <param name="ro">横座標</param>
    /// <param name="co">縦座標</param>
    public void CellOpen(int ro, int co)
    {
        for (int c = -1; c < 2; c++)
        {
            for (int r = -1; r < 2; r++)
            {
                if (ro + r == -1 || co + c == -1 || ro + r == _cells.GetLength(0) || co + c == _cells.GetLength(1))
                {
                    continue;
                }
                else if (r == 0 && c == 0)
                {
                    continue;
                }
                else if (_cells[ro + r, co + c].Status != Status.Open)
                {
                    if (_cells[ro + r, co + c].Status == Status.Flag)//旗だったら展開しない
                    {
                        continue;
                    }
                    _cells[ro + r, co + c].Status = (Status)3;
                }
            }
        }
    }
    /// <summary>
    /// 旗が立てられた数を受け取る
    /// </summary>
    /// <param name="f"></param>
    public void Flag(int f)
    {
        m_flag += f;       
    }
    /// <summary>
    /// 爆弾セルに旗が置かれた数を受け取る
    /// </summary>
    /// <param name="f"></param>
    public void BombF(int f)
    {
        m_bombFlag += f;
        if (m_bombFlag == m_bomb)
        {
            GameClear();            
        }
    }
    public void BombOtherThan(int f)
    {
        m_bombOtherThan += f;
        if (m_row * m_col - m_bombOtherThan == m_bomb)
        {
            GameClear();
        }
    }
    /// <summary>
    /// 旗数の上限を制限
    /// </summary>
    /// <returns></returns>
    public bool Fl()
    {
        if (m_flag < m_bomb)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GameClear()
    {
        Debug.Log("ゲームクリア");
    }

    public void GameOver()
    {
        Debug.Log("ゲームオーバー");
    }

    private void AddMine(int r, int c)
    {
        var left = c - 1;
        var right = c + 1;
        var top = r - 1;
        var bottom = r + 1;
        if (top >= 0)
        {
            if (left >= 0 && _cells[top, left].CellState != CellState.Mine) { _cells[top, left].CellState++; }
            if (_cells[top, c].CellState != CellState.Mine) { _cells[top, c].CellState++; }
            if (right < m_col && _cells[top, right].CellState != CellState.Mine) { _cells[top, right].CellState++; }
        }
        if (left >= 0 && _cells[r, left].CellState != CellState.Mine) { _cells[r, left].CellState++; }
        if (right < m_col && _cells[r, right].CellState != CellState.Mine) { _cells[r, right].CellState++; }
        if (bottom < m_row)
        {
            if (left >= 0 && _cells[bottom, left].CellState != CellState.Mine) { _cells[bottom, left].CellState++; }
            if (_cells[bottom, c].CellState != CellState.Mine) { _cells[bottom, c].CellState++; }
            if (right < m_col && _cells[bottom, right].CellState != CellState.Mine) { _cells[bottom, right].CellState++; }
        }

    }

    private int GetMineCount(int r, int c)
    {
        var left = c - 1;
        var right = c + 1;
        var top = r - 1;
        var bottom = r + 1;

        var count = 0;
        if (top >= 0)
        {
            if (left >= 0 && _cells[top, left].CellState == CellState.Mine) { count++; }
            if (_cells[top, c].CellState == CellState.Mine) { count++; }
            if (right < m_col && _cells[top, right].CellState == CellState.Mine) { count++; }
        }
        if (left >= 0 && _cells[r, left].CellState == CellState.Mine) { count++; }
        if (right < m_col && _cells[r, right].CellState == CellState.Mine) { count++; }
        if (bottom < m_row)
        {
            if (left >= 0 && _cells[bottom, left].CellState == CellState.Mine) { count++; }
            if (_cells[bottom, c].CellState == CellState.Mine) { count++; }
            if (right < m_col && _cells[bottom, right].CellState == CellState.Mine) { count++; }
        }
        return count;
    }
    void Update()
    {

    }
}
