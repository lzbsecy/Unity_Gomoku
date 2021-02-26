using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessType
{
    Null,
    black,
    white
}
public enum AI
{
    NULL,
    Low,
    High
}
public class GameTree
{
    public GameTree[] child;
    public GameTree parent;
    public int depth;
    public int posX;
    public int posY;
    public int score;
    public int chess;
    public int solve;
}
public struct ScorePos
{
    public int score;
    public int posX;
    public int posY;
}
class ScoreComparer : IComparer<ScorePos>
{
    public int Compare(ScorePos x, ScorePos y)
    {
        if (x.score < y.score) return 1;    //从大到小排序
        else if (x.score > y.score) return -1;
        else return 0;
    }
}
public class GameStatus : MonoBehaviour
{
    public ChessType turn;
    public int[,] chessboard;
    public int round;
    public List<GameObject> chessPieces;

    public AI ai;
    public bool IsOver;

    // Start is called before the first frame update
    void Start()
    {

        chessboard = new int[15,15];
        turn = ChessType.black;
        round = 0;
        IsOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetChess(int posX, int posY)
    {
        return chessboard[posX, posY];
    }
    public void SetChess(int posX,int posY,int type)
    {
        chessboard[posX, posY] = type;
    }
    public ChessType GetTurn()
    {
        return turn;
    }
    public void SetTurn(ChessType set)
    {
        if (set == ChessType.black) turn = ChessType.black;
        else turn = ChessType.white;
    }

}
