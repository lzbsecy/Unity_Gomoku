using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetGameSystem : NetworkBehaviour
{

    public GameObject black;
    public GameObject white;

    private NetGameStatus status;

    // Start is called before the first frame update
    void Start()
    {
        status = gameObject.GetComponent<NetGameStatus>();

    }

    // Update is called once per frame
    void Update()
    {
        if (status.IsOver)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// 放置棋子
    /// </summary>
    /// <param name="pos"></param>
    public void Put(Vector2 pos)
    {
        if (status.GetChess((int)pos.x, (int)pos.y) == 0)
        {
            if (status.GetTurn() == ChessType.black)
            {
                status.SetChess((int)pos.x, (int)pos.y, 1);
                GameObject chess = Instantiate(black, pos, Quaternion.identity);
                status.chessPieces.Add(chess);
                NetworkServer.Spawn(chess);

                if (CheckGameOver(pos))
                    GameOverEvent(status.turn);
                status.SetTurn(ChessType.white);
            }
            else if (status.GetTurn() == ChessType.white)
            {
                status.SetChess((int)pos.x, (int)pos.y, 2);
                GameObject chess = Instantiate(white, pos, Quaternion.identity);
                status.chessPieces.Add(chess);
                NetworkServer.Spawn(chess);

                if (CheckGameOver(pos))
                    GameOverEvent(status.turn);
                status.SetTurn(ChessType.black);
            }
        }
    }


    /// <summary>
    /// 检测游戏是否结束
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    [Server]
    public bool CheckGameOver(Vector2 pos)
    {
        int sum = 0;

        sum = Check(pos, 1, 0, sum);

        sum = Check(pos, 0, 1, sum);

        sum = Check(pos, 1, -1, sum);

        sum = Check(pos, 1, 1, sum);

        if (sum >= 5)
            return true;
        return false;
    }

    public int Check(Vector2 pos, int dirX, int dirY, int sum)
    {
        if (sum == 5) return 5;
        int i = (int)pos.x, j = (int)pos.y;
        int[] key = { (int)pos.x, (int)pos.y };
        while (status.chessboard[i, j] == status.chessboard[key[0], key[1]])
        {
            if (sum < 5) sum++;
            //Debug.Log(i + " , " + j + " : " + status.checkerboard[i, j] + " = " + status.checkerboard[key[0], key[1]] + " ++");
            i = i + dirX;
            j = j + dirY;
            if (i > 14 || j > 14 || i < 0 || j < 0)
                break;
        }
        i = (int)pos.x;
        j = (int)pos.y;
        while (status.chessboard[i, j] == status.chessboard[key[0], key[1]])
        {
            i = i - dirX;
            j = j - dirY;
            if (i > 14 || j > 14 || i < 0 || j < 0)
                break;
            if (status.chessboard[i, j] == status.chessboard[key[0], key[1]] && sum < 5) sum++;
            //Debug.Log(i + " , " + j + " : " +status.checkerboard[i, j] + " = " + status.checkerboard[key[0], key[1]] + " ++"); 
        }
        //Debug.Log(sum);
        if (sum < 5) return 0;
        else return 5;
    }

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    /// <param name="type"></param>
    public void GameOverEvent(ChessType type)
    {
        if (type == ChessType.black)
        {
            Debug.Log("黑棋胜");
            status.IsOver = true;
        }
        else if (type == ChessType.white)
        {
            Debug.Log("白棋胜");
            status.IsOver = true;
        }

        Debug.Log("游戏结束");
    }

    /// <summary>
    /// 悔棋
    /// </summary>
    public void BackMove()
    {

        if (status.chessPieces.Count != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = status.chessPieces[status.chessPieces.Count - 1];
                status.chessPieces.RemoveAt(status.chessPieces.Count - 1);
                status.chessboard[(int)temp.transform.position.x, (int)temp.transform.position.y] = 0;
                Destroy(temp);
                if (status.chessPieces.Count == 0)
                {
                    status.SetTurn(ChessType.black);
                    status.IsOver = false;
                    break;
                }
            }
            if(status.chessPieces.Count!=0)
            {
                if (!CheckGameOver(new Vector2((int)status.chessPieces[status.chessPieces.Count - 1].transform.position.x
                , (int)status.chessPieces[status.chessPieces.Count - 1].transform.position.y)))
                {
                    status.IsOver = false;
                }
            }
        }
        else
        {
            Debug.Log("not your round or chessPieces not enough");
        }
    }



}
