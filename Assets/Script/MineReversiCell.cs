using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineReversiCell : MonoBehaviour
{
    [SerializeField] GameObject m_piece = null;
    [SerializeField] private Text m_view = null;
    [SerializeField] private CellState m_cellState = CellState.None;
    [SerializeField] State m_state = State.Close;
    [SerializeField] private PieceState m_pieceState = PieceState.Black;
    [SerializeField] GameObject m_button = null;
    public int m_row;
    public int m_col;
    [SerializeField] Image m_image = null;

    private GameObject mine;

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

    public CellState CellState
    {
        get => m_cellState;
        set
        {
            m_cellState = value;
            OnCellStateChanged();
        }
    }

    private void OnValidate()
    {
        OnPieceColorChanged();
        OnPiecePutChanged();
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        if (m_view == null)
        {
            return;
        }
        if (m_cellState == CellState.None)
        {
            m_view.text = "";
        }
        else if (m_cellState == CellState.Mine)
        {
            m_view.text = "X";
            m_view.color = Color.red;
        }
        else
        {
            m_view.text = ((int)m_cellState).ToString();
            m_view.color = Color.blue;
        }
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
            m_button.gameObject.SetActive(false);
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
            MineReversiField field = FindObjectOfType<MineReversiField>();
            field.CellCoordinate(m_row, m_col);
            field.PieceChangeAll(m_row, m_col);
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
        m_button.gameObject.SetActive(false);
    }

    void Start()
    {

    }
    void Update()
    {

    }
}
