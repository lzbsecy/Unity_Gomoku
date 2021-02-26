using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour
{
    
    RaycastHit hit;
    //public static int chess = 1;
    [SyncVar]
    public ChessType turn;

    private NetGameSystem system;
    private NetGameStatus status;
    private GameObject backMoveBtn;

    void Start()
    {
        system = GameObject.Find("netboard").GetComponent<NetGameSystem>();
        status = GameObject.Find("netboard").GetComponent<NetGameStatus>();
        backMoveBtn = GameObject.Find("BackMove").gameObject;

        GameObject.Find("BackMove").GetComponent<Button>().onClick.AddListener(BackMove);

        if(isServer)
        {
            if(status.chess<3)
            {
                turn = (ChessType)status.chess;
                status.chess++;
            }
            else
            {
                turn = ChessType.Null;
            }
        }
    }
    void Update()
    {
        IsGameOver();
    }

    void FixedUpdate()
    {
        PutChess();
    }

    /// <summary>
    /// 下棋
    /// </summary>
    public void PutChess()
    {
        if (turn == status.turn)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if(isLocalPlayer)
                    {
                        Vector2 pos = new Vector2((int)(hit.point.x + 0.5f), (int)(hit.point.y + 0.5f));
                        //Debug.Log((int)(hit.point.x + 0.5f) + " , " + (int)(hit.point.y + 0.5f));
                        CmdPut(pos);
                    }
                }
            }
        }
    }
    public void BackMove()
    {
        if(turn ==status.turn)
        {
            CmdBackMove();
        }
    }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    public void IsGameOver()
    {
        if(checkOver())
        {
           
        }
    }

    public bool checkOver()
    {
        if (status.IsOver)
        {
            Time.timeScale = 0;
            return true;
        }
        else
        {
            Time.timeScale = 1;
            return false;
        }
    }

    [Command]
    public void CmdPut(Vector2 pos)
    {
        system.Put(pos);
    }
    [Command]
    public void CmdBackMove()
    {
        system.BackMove();
    }
}
