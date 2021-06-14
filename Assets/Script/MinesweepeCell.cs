using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,

    Mine = -1,
}

public enum Status
{
    Close = 0,
    Open = 1,
    Flag = 2,
    NoneOpen = 3,
}

public class MinesweepeCell : MonoBehaviour
{
    [SerializeField] private Text m_view = null;
    [SerializeField] private CellState m_cellState = CellState.None;
    [SerializeField] Button m_button = null;
    public int row;
    public int col;
    [SerializeField] private Status m_status = Status.Close;
    [SerializeField] Image m_image = null;
    [SerializeField] GameObject m_flag = null;

    private GameObject mine;

    public CellState CellState
    {
        get => m_cellState;
        set
        {
            m_cellState = value;
            OnCellStateChanged();
        }
    }

    public Status Status
    {
        get => m_status;
        set
        {
            m_status = value;
            OnCellflagChanged();
        }
    }

    void Start()
    {
        mine = GameObject.Find("Stage");
    }
    void Update()
    {

    }

    private void OnValidate()
    {
        OnCellStateChanged();
        OnCellflagChanged();
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

    private void OnCellflagChanged()
    {
        if (m_status == Status.Close)
        {
            m_flag.SetActive(false);
        }
        else if (m_status == Status.Flag)
        {
            m_flag.SetActive(true);
        }
        else if (m_status == Status.Open)
        {
            Open();
        }
        else if (m_status == Status.NoneOpen)
        {
            NoneOpen();
        }
    }
    /// <summary>
    /// セル番号を受け取る
    /// </summary>
    /// <param name="r">横</param>
    /// <param name="c">縦</param>
    public void GetCoordinate(int r, int c)
    {
        row = r;
        col = c;
    }
    /// <summary>
    /// セルを展開する
    /// </summary>
    public void Open()
    {
        if (this.m_status != Status.Flag)
        {
            m_button.gameObject.SetActive(false);
            this.m_status = Status.Open;
            Debug.Log(row + " " + col + "が開いた");
            if (this.m_cellState == CellState.Mine)
            {
                m_image.color = Color.red;
                mine.GetComponent<MinesweeperField>().GameOver();
            }
            else
            {
                MinesweeperField field = FindObjectOfType<MinesweeperField>();
                if (field)
                {
                    field.CellCoordinate(row, col);
                    field.BombOtherThan();
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void NoneOpen()
    {
        m_button.gameObject.SetActive(false);
        this.m_status = Status.Open;
        Debug.Log(row + " " + col + "が開いた");
        MinesweeperField field = FindObjectOfType<MinesweeperField>();
        if (field)
        {
            if (this.m_cellState == CellState.None)
            {
                field.CellOpen(row, col);
            }
        }
    }

    /// <summary>
    /// 旗をたてる
    /// </summary>
    public void Flag()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (this.m_status == Status.Close)
            {
                bool f = mine.GetComponent<MinesweeperField>().Fl();
                if (f == true)
                {
                    m_flag.SetActive(true);
                    this.m_status = Status.Flag;
                    MinesweeperField field = FindObjectOfType<MinesweeperField>();
                    if (field)
                    {
                        field.Flag(1);
                    }
                    if (this.m_cellState == CellState.Mine)
                    {
                        field.BombF(1);
                    }
                }
                else
                {
                    Debug.Log("もう旗がない");
                }

            }
            else
            {
                m_flag.SetActive(false);
                this.m_status = Status.Close;
                MinesweeperField field = FindObjectOfType<MinesweeperField>();
                if (field)
                {
                    field.Flag(-1);
                }
                if (this.m_cellState == CellState.Mine)
                {
                    field.BombF(-1);
                }
            }
        }
    }
    public void GameEnd()
    {
        m_button.gameObject.SetActive(false);
    }

}
