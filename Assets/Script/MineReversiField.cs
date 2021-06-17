using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MineReversiField : MonoBehaviour
{
    [SerializeField] MineReversiCell m_cellPrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 0;
    [SerializeField] int m_col = 0;
    [SerializeField] int m_bomb = 0;
    private MineReversiCell[,] _cells;
    [SerializeField] Text m_WhiteCount = null;
    [SerializeField] Text m_BlackCount = null;
    [SerializeField] Text m_order = null;
    [SerializeField] Text m_text = null;
    [SerializeField] GameObject m_panel = null;
    [SerializeField] GameObject m_Difficulty = null;
    [SerializeField] GameObject m_lizard = null;
    [SerializeField] Text m_lizardJudgment = null;
    int m_DifficultyPattern;
    bool m_cheat = true;
    bool m_turn = true;
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
        _cells = new MineReversiCell[m_row, m_col];

        //セルを生成
        for (int col = 0; col < m_col; col++)
        {
            for (int row = 0; row < m_row; row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                _cells[row, col] = cell;
                cell.GetCoordinate(row, col);
                if (row == 3 + m_DifficultyPattern && col == 3 + m_DifficultyPattern || row == 4 + m_DifficultyPattern && col == 4 + m_DifficultyPattern)
                {
                    var _piece = _cells[row, col];
                    _piece.State = State.StateOpen;
                    _piece.PieceState = PieceState.White;
                }
                if (row == 4 + m_DifficultyPattern && col == 3 + m_DifficultyPattern || row == 3 + m_DifficultyPattern && col == 4 + m_DifficultyPattern)
                {
                    var _piece = _cells[row, col];
                    _piece.State = State.StateOpen;
                    _piece.PieceState = PieceState.Black;
                }
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
            if (cells.CellState != CellState.Mine && cells.PieceState == PieceState.None)//もし選ばれたセルが爆弾持ちでなかったら
            {
                cells.CellState = CellState.Mine;//爆弾を付与
            }
            else
            {
                i--;//そうでなかったら抽選をやり直す
            }
        }

        //数字振り2
        for (int coln = 0; coln < _cells.GetLength(1); coln++)
        {
            for (int rown = 0; rown < _cells.GetLength(0); rown++)
            {
                if (_cells[rown, coln].CellState != CellState.Mine)
                {
                    continue;
                }
                else
                {
                    for (int c = -1; c < 2; c++)
                    {
                        for (int r = -1; r < 2; r++)
                        {
                            if (rown + r == -1 || coln + c == -1 || rown + r == _cells.GetLength(0) || coln + c == _cells.GetLength(1))
                            {
                                continue;
                            }
                            else if (_cells[rown + r, coln + c].CellState != CellState.Mine)
                            {
                                _cells[rown + r, coln + c].CellState += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ChangeTurn(int r, int c)
    {
        if (m_turn == true)
        {
            _cells[r, c].State = State.Open;
            _cells[r, c].PieceState = PieceState.White;
            m_turn = false;
            m_order.text = "黒の番";
        }
        else
        {
            _cells[r, c].State = State.Open;
            _cells[r, c].PieceState = PieceState.Black;
            m_turn = true;
            m_order.text = "白の番";
        }
        SearchAll();
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
                else if (_cells[ro + r, co + c].State == State.Open)
                {
                    if (_cells[ro + r, co + c].CellState == CellState.None)
                    {
                        _cells[ro + r, co + c].State = (State)3;
                    }
                }
            }
        }
    }

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
                if (ChangeCount != 0 && _cells[r, c].State == State.Close)
                {
                    //そのセルの色を変える
                    _cells[r, c].GetComponent<Image>().color = Color.gray;
                    AllChangeCount++;
                }
                else
                {
                    _cells[r, c].GetComponent<Image>().color = Color.white;
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
            if (_cells[row, col].PieceState == PieceState.None)
            {
                break;
            }
            if (m_turn == true)
            {
                if (_cells[row, col].PieceState == PieceState.Black)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (_cells[row, col].PieceState == PieceState.White)
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
                if (_cells[row, col].PieceState == PieceState.White)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (_cells[row, col].PieceState == PieceState.Black)
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
            if (_cells[row, col].PieceState == PieceState.None)
            {
                break;
            }
            if (m_turn == true)
            {
                if (_cells[row, col].PieceState == PieceState.Black)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (_cells[row, col].PieceState == PieceState.White)
                {
                    if (checkCount != 0)
                    {
                        int ChangeRow = r + moveR;
                        int ChangeCol = c + moveC;
                        while (!(ChangeRow == row && ChangeCol == col))
                        {
                            _cells[ChangeRow, ChangeCol].PieceState = PieceState.White;
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
                if (_cells[row, col].PieceState == PieceState.White)
                {
                    row += moveR;
                    col += moveC;
                    checkCount++;
                }
                else if (_cells[row, col].PieceState == PieceState.Black)
                {
                    if (checkCount != 0)
                    {
                        int ChangeRow = r + moveR;
                        int ChangeCol = c + moveC;
                        while (!(ChangeRow == row && ChangeCol == col))
                        {
                            _cells[ChangeRow, ChangeCol].PieceState = PieceState.Black;
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
    /// <summary>
    /// 石の数を更新し、必要があればゲームを終了させる。
    /// </summary>
    void Aggregate()
    {
        int white = 0;
        int black = 0;
        int none = 0;
        foreach (var i in _cells)
        {
            if (i.PieceState == PieceState.Black)
            {
                black++;
            }
            else if (i.PieceState == PieceState.White)
            {
                white++;
            }
            else
            {
                none++;
            }
        }
        m_WhiteCount.text = "白：" + white;
        m_BlackCount.text = "黒：" + black;
        if (none == 0 || white == 0 || black == 0)
        {
            GameSet(white, black);
        }
    }

    void GameSet(int white, int black)
    {
        m_lizard.SetActive(true);
        if (white > black)
        {
            m_lizardJudgment.text = "白の勝ち！";
        }
        else
        {
            m_lizardJudgment.text = "黒の勝ち！";
        }
    }

    void Update()
    {
        if (m_cheat == true)
        {
            if (Input.GetKey("w") && Input.GetMouseButtonDown(1))
            {
                StartCoroutine("WhiteCheat");
                m_cheat = false;
            }
            if (Input.GetKey("b") && Input.GetMouseButtonDown(1))
            {
                StartCoroutine("BlackCheat");
                m_cheat = false;
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

    public void Easy()
    {
        m_Difficulty.SetActive(false);
        m_row = 6;
        m_col = 6;
        m_bomb = 4;
        m_DifficultyPattern = -1;
        Start();

    }

    public void Normal()
    {
        m_row = 8;
        m_col = 8;
        m_bomb = 6;
        m_DifficultyPattern = 0;
        Start();
        m_Difficulty.SetActive(false);
    }

    public void Hard()
    {
        m_row = 10;
        m_col = 10;
        m_bomb = 8;
        m_DifficultyPattern = 1;
        Start();
        m_Difficulty.SetActive(false);
    }

}
