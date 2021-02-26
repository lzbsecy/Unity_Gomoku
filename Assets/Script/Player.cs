using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    RaycastHit hit;
    public Camera cam;
    public GameObject chessboard;

    public ChessType turn;

    private GameSystem system;
    private GameStatus status;
    // Start is called before the first frame update
    void Start()
    {
        system = chessboard.GetComponent<GameSystem>();
        status = chessboard.GetComponent<GameStatus>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PutChess()
    {
        if (turn == status.turn)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Vector2 pos = new Vector2((int)(hit.point.x + 0.5f), (int)(hit.point.y + 0.5f));
                    //Debug.Log((int)(hit.point.x + 0.5f) + " , " + (int)(hit.point.y + 0.5f));
                    system.Put(pos);
                }
            }
        }
    }

    public bool TurnSelf()
    {
        if (turn == status.GetTurn())
        {
            if (turn == ChessType.black)
            {
                if (status.round % 2 == 1)
                    return true;
                else return false;
            }
            else if (turn == ChessType.white)
            {
                if (status.round % 2 == 0)
                    return true;
                else return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
