using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PieceState
{
    None = 0,
    Black = 1,
    White = 2,
}

public enum State
{
    Close = 0,
    Open = 1,
    StateOpen = 2,
}

public class ReversiPiece : MonoBehaviour
{
    [SerializeField] GameObject m_piece = null;
    [SerializeField] PieceState m_pieceState = PieceState.Black;
    [SerializeField] State m_state = State.Close;
    public int m_row;
    public int m_col;
    [SerializeField] Image m_image = null;
    private GameObject reversi;

    public PieceState PieceState
    {
        get => m_pieceState;
        set
        {
            m_pieceState = value;
            OnPieceColorChanged();
        }
    }
    public State State
    {
        get => m_state;
        set
        {
            m_state = value;
            OnPiecePutChanged();
        }
    }

    private void OnValidate()
    {
        OnPieceColorChanged();
        OnPiecePutChanged();
    }


    private void OnPieceColorChanged()
    {
        m_image = transform.Find("Piece").gameObject.GetComponent<Image>();
        if (m_pieceState == PieceState.Black)
        {
            m_image.color = Color.black;
        }
        else if (m_pieceState == PieceState.White)
        {
            m_image.color = Color.white;
        }
    }
    private void OnPiecePutChanged()
    {
        if (m_state == State.Close)
        {

        }
        else if (m_state == State.Open)
        {
            m_piece.gameObject.SetActive(true);
        }
        else if (m_state == State.StateOpen)
        {
            StateOpen();
        }
    }

    /// <summary>
    /// 座標を受け取る
    /// </summary>
    /// <param name="r">横座標</param>
    /// <param name="c">縦座標</param>
    public void GetCoordinate(int r, int c)
    {
        m_row = r;
        m_col = c;
    }

    /// <summary>
    /// マスを展開する
    /// </summary>
    public void Open()
    {
        if (this.m_state == State.Close)
        {
            ReversiField field = FindObjectOfType<ReversiField>();
            field.PieceChangeAll(m_row, m_col);
            //reversi.GetComponent<ReversiField>().PieceChangeAll(m_row, m_col);
        }
        else
        {
            Debug.Log("ここには置けない");
        }
    }

    public void StateOpen()
    {
        this.m_state = State.Open;
        m_piece.gameObject.SetActive(true);
    }

    void Start()
    {

    }
    void Update()
    {

    }
}
