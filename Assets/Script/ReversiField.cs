using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReversiField : MonoBehaviour
{
    [SerializeField] ReversiPiece m_piecePrefab;//セルのプレハブ
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] int m_row = 8;//横の長さ
    [SerializeField] int m_col = 8;//縦の長さ
    [SerializeField] Text m_WhiteCount = null;
    [SerializeField] Text m_BlackCount = null;
    [SerializeField] Text m_order = null;
    private ReversiPiece[,] m_piece;//ステージの配列
    bool m_turn = true;
    [SerializeField] GameObject m_lizard = null;
    [SerializeField] Text m_lizardJudgment = null;
    bool m_cheat = true;
    [SerializeField] string m_GameRecord = "";

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
        m_order.text = "白の番";
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
            m_order.text = "黒の番";
        }
        else
        {
            m_piece[r, c].State = State.Open;
            m_piece[r, c].PieceState = PieceState.Black;
            m_turn = true;
            m_order.text = "白の番";
        }
        SearchAll();
    }

    /// <summary>
    /// 置ける場所を探す全てのパターン
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
    /// <summary>
    /// 置ける場所を探す
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="moveR"></param>
    /// <param name="moveC"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 石の数を更新し、必要があればゲームを終了させる。
    /// </summary>
    void Aggregate()
    {
        int white = 0;
        int black = 0;
        int none = 0;
        foreach (var i in m_piece)
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
        if (white == black)
        {
            m_lizardJudgment.text = "引き分け！";
        }
        else if (white > black)
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
            if (Input.GetKey("g") && Input.GetMouseButtonDown(1))
            {
                StartCoroutine("GameRecordCheat");
                m_cheat = false;
            }
        }
    }
    /// <summary>
    /// 棋譜の実行
    /// </summary>
    /// <returns></returns>
    IEnumerator GameRecordCheat()
    {
        int[] conversion = new int[m_GameRecord.Length];
        for (int i = 0; i < m_GameRecord.Length; i++)
        {
            char num = m_GameRecord[i];
            if (num >= '1' && num <= '8')
            {
                conversion[i] = num - '1';
            }
            else if(num >= 'a' && num <= 'h')
            {
                conversion[i] = num - 'a';
            }
        }
        for (int i = 1; i < conversion.Length; i += 2)
        {
            PieceChangeAll(conversion[i - 1], conversion[i]);
            yield return new WaitForSeconds(0.5f);
        }
    }
    /// <summary>
    /// 黒勝利の最短終了
    /// </summary>
    /// <returns></returns>
    IEnumerator BlackCheat()
    {
        PieceChangeAll(5, 3);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(5, 2);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(4, 2);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(5, 4);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(4, 5);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(3, 6);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(3, 5);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(3, 2);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(2, 4);
    }
    /// <summary>
    /// 白勝利の最短終了
    /// </summary>
    /// <returns></returns>
    IEnumerator WhiteCheat()
    {
        PieceChangeAll(5, 3);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(3, 2);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(2, 3);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(5, 4);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(4, 1);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(5, 2);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(6, 3);
        yield return new WaitForSeconds(0.5f);
        PieceChangeAll(4, 2);
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
