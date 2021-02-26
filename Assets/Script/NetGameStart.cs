using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetGameStart : NetworkBehaviour
{
    public GameObject chessboard;

    void Start()
    {
        if(isServer)
        {
            GameObject board = Instantiate(chessboard, new Vector2(7, 7), Quaternion.identity);
            NetworkServer.Spawn(chessboard);
        }
    }


}
