using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetGameStatus : NetworkBehaviour
{
    [SyncVar]
    public ChessType turn;
    [SyncVar]
    public int round;
    [SyncVar]
    public bool IsOver;

    public int[,] chessboard;
    public List<GameObject> chessPieces;

    public int chess = 1;
    // Start is called before the first frame update
    void Start()
    {
        chessboard = new int[15, 15];
        turn = ChessType.black;
        round = 0;
        IsOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public override void OnStartServer()
    {
        turn = ChessType.black;
        chessPieces.Clear();
        chess = 1;
        chessboard = new int[15, 15];
    }
    [Server]
    public int GetChess(int posX, int posY)
    {
        return chessboard[posX, posY];
    }
    [Server]
    public void SetChess(int posX, int posY, int type)
    {
        chessboard[posX, posY] = type;
    }
    [Server]
    public ChessType GetTurn()
    {
        return turn;
    }
    [Server]
    public void SetTurn(ChessType set)
    {
        if (set == ChessType.black) turn = ChessType.black;
        else turn = ChessType.white;
    }

}
