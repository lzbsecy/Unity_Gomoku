using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    RaycastHit hit;
    public Camera cam;
    public GameObject chessboard;

    public bool isPlayer;
    public ChessType turn;
    public AI aiLevel;
    public Dictionary<string, int> valueTable = new Dictionary<string, int>();

    public int width = 5;  //每层向下拓展的宽度
    public int depth = 7;  //总层数（必须是奇数）

    private GameSystem system;
    private GameStatus status;
    ScoreComparer comparer = new ScoreComparer();

    void Start()
    {
        system = chessboard.GetComponent<GameSystem>();
        status = chessboard.GetComponent<GameStatus>();

        ScoreStart();
    }

    void Update()
    {
        if(aiLevel!=status.ai)
        {
            aiLevel = status.ai;
        }

    }


    public void AIPut()
    {
        if (isPlayer)
        {
            PutChess();
        }
        else
        {
            if (aiLevel == AI.Low)
            {
                AiPlayerLow();
            }
            else if (aiLevel == AI.High)
            {
                AiPlayerHigh();
            }
        }
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
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Vector2 pos = new Vector2((int)(hit.point.x + 0.5f), (int)(hit.point.y + 0.5f));
                    //Debug.Log((int)(hit.point.x + 0.5f) + " , " + (int)(hit.point.y + 0.5f));
                    Debug.Log(pos + " " + CountValue(pos));
                    system.Put(pos);
                }
            }
        }
    }

    #region AILow

    public void AiPlayerLow()
    {

        if (turn == status.turn)
        {
            if(status.chessPieces.Count==0)
            {
                system.Put(new Vector2(7, 7));
            }
            else
            {
                system.Put(FindBestPos());
            }
        }
    }
    public Vector2 FindBestPos()
    {
        Vector2 bestPos = new Vector2(1,1);
        int maxValue = 0, tempValue = 0;

        for(int i=0;i<15;i++)
        {
            for(int j =0;j<15;j++)
            {
                if(status.chessboard[i,j]==0)
                {
                    Vector2 pos = new Vector2(i, j);
                    tempValue = CountValue(pos);
                    if (maxValue < tempValue)
                    {
                        maxValue = tempValue;
                        bestPos = pos;
                    }
                }
            }
        }
        Debug.Log("BestPos" + bestPos + "MaXPos" + maxValue);
        return bestPos;
    }
    public int CountValue(Vector2 pos)
    {
        int sum = 0;

        //单方向计算分值完毕，结合角度另外计算
        sum = sum + Compare(pos, 1, 0, sum);

        sum = sum + Compare(pos, 1, -1, sum);

        sum = sum + Compare(pos, 0, 1, sum);

        sum = sum + Compare(pos, 1, 1, sum);

        return sum;
    }
    public int Compare(Vector2 pos, int dirX, int dirY, int sum)
    {
        string list = "1";
        int sumValue = 0;
        int i = (int)pos.x, j = (int)pos.y;
        int count = 0;
        do
        {
            i = i + dirX;
            j = j + dirY;
            if (i > 14 || j > 14 || i < 0 || j < 0)
                break;

            //添加末尾字符串，如堵或者空
            if (status.chessboard[i, j] != (int)turn)
            {
                if (status.chessboard[i, j] == 0)
                {
                    list = list + "0";
                    count++;
                }
                else
                {
                    list = list + "2";
                }
            }
            else
            {
                list = list + "1";
            }
        }
        while (status.chessboard[i, j] != 0 || count < 2);

        i = (int)pos.x;
        j = (int)pos.y;
        count = 0;

        do
        {
            i = i - dirX;
            j = j - dirY;
            if (i > 14 || j > 14 || i < 0 || j < 0)
                break;

            //添加末尾字符串，如堵或者空
            if (status.chessboard[i, j] != (int)turn)
            {
                if (status.chessboard[i, j] == 0)
                {
                    list = "0" + list;
                    count++;
                }
                else
                {
                    list = "2" + list;
                }
            }
            else
            {
                list = "1" + list;
            }
        }
        while (status.chessboard[i, j] != 0 || count < 2);


        //比对字符串获取分值
        foreach (var obj in valueTable)
        {
            if (list.Contains(obj.Key))
            {
                if (list[1] != '0' || list[list.Length - 2] != '0') 
                {
                    if(list[0]==0 || list[list.Length-1]==0)
                    {
                        if (list.Length - obj.Key.Length < 2)
                        {
                            sumValue = sumValue + obj.Value;
                        }
                    }
                }
                else
                {
                    if (list.Length - obj.Key.Length < 3)
                    {
                        sumValue = sumValue + obj.Value;
                    }
                }
            }
        }
        Debug.Log(list);
        return sumValue;
    }
    //1指自己，2指对手，0指空白
    public void ScoreStart()
    {
        //攻
        //五连
        valueTable.Add("0111110", 1000000);
        valueTable.Add("02111110", 1000000);
        valueTable.Add("01111120", 1000000);
        //四连+两边空
        valueTable.Add("011110", 50000);
        valueTable.Add("0211110", 10000);
        //三连+两边空
        valueTable.Add("01110", 10000);
        //三连+一边空
        valueTable.Add("021110", 2500);
        valueTable.Add("011120", 2500);
        //二连+非空
        valueTable.Add("0110", 5000);
        //二连+一边空
        valueTable.Add("02110", 10);
        valueTable.Add("01120", 10);
        //二连+两边堵
        valueTable.Add("2112", 5);
        //一连+两边空
        valueTable.Add("010", 1);
        //防
        //五连
        valueTable.Add("02222100", 10000000);
        valueTable.Add("01222210", 10000000);

        valueTable.Add("02221200", 10000000);
        valueTable.Add("00222120", 10000000);

        valueTable.Add("02212200", 10000000);
        valueTable.Add("02212210", 10000000);

        valueTable.Add("02122200", 10000000);
        valueTable.Add("01222200", 10000000);

        //四联
        valueTable.Add("022210", 20000);
        valueTable.Add("122201", 20000);
        valueTable.Add("102221", 20000);
        valueTable.Add("122210", 2500);

        valueTable.Add("022120", 20000);
        valueTable.Add("022121", 2500);
        valueTable.Add("122120", 2500);
        //valueTable.Add("122121", 1000);

        valueTable.Add("021220", 20000);
        valueTable.Add("021221", 5000);
        valueTable.Add("121220", 5000);
        // valueTable.Add("121221", 1000);

        valueTable.Add("012220", 20000);
        valueTable.Add("012221", 2500);
        //valueTable.Add("112221", 1000);

        //三连
        valueTable.Add("02210", 5000);
        valueTable.Add("02120", 5000);
        valueTable.Add("01220", 5000);
        //一连
        valueTable.Add("0210", 10);
        valueTable.Add("0120", 10);

    }

    #endregion


    #region AIHigh
    public void AiPlayerHigh()
    {
        if (turn == status.turn)
        {
            if (status.chessPieces.Count == 0)
            {
                system.Put(new Vector2(7, 7));
            }
            else
            {
                system.Put(FindBestScorePos());
            }
        }
    }

    public Vector2 FindBestScorePos()
    {
        GameTree root = new GameTree();
        root.depth = 1;
        root.chess = 1; //黑的刚走完
        root.score = 0;
        GameTree(status.chessboard, root);//博弈树递归
        //递归求出分数
        UpdateTreeScore(root);
        int ans = 0;
        int maxScore = 0;
        for (int i = 0; i < 0; i++)
        {
            if (root.child[i].score > maxScore)
            {
                maxScore = root.child[i].score;
                ans = i;
            }
        }
        int posi = root.child[ans].posX;
        int posj = root.child[ans].posY;
        Vector2 bestPos = new Vector2(posi, posj);
        return bestPos;
    }

    private void GetScore(int i, int j, ref int black, ref int white, int[,] map)
    {
        //若放黑色
        int chess = 1;
        int s1 = 0, s2 = 0, s3 = 0, s4 = 0; //四个方向的棋子的数量
        //横向检查
        int a = 0, b = 0;   //两端连续棋子数量
        for (int k = i - 1; k >= 0; k--)
        {
            if (map[k, j] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int k = i + 1; k < 15; k++)
        {
            if (map[k, j] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s1 = a + b + 1;
        //纵向检查
        a = b = 0;
        for (int k = j - 1; k >= 0; k--)
        {
            if (map[i, k] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int k = j + 1; k < 15; k++)
        {
            if (map[i, k] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s2 = a + b + 1;
        //斜左下
        a = b = 0;
        for (int m = i - 1, n = j - 1; m >= 0 && n >= 0; m--, n--)
        {
            if (map[m, n] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int m = i + 1, n = j + 1; m < 15 && n < 15; m++, n++)
        {
            if (map[m, n] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s3 = a + b + 1;
        //斜右下
        a = b = 0;
        for (int m = i - 1, n = j + 1; m >= 0 && n < 15; m--, n++)
        {
            if (map[m, n] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int m = i + 1, n = j - 1; m < 15 && n >= 0; m++, n--)
        {
            if (map[m, n] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s4 = a + b + 1;
        black = GetScore(s1, -1) + GetScore(s2, -1) + GetScore(s3, -1) + GetScore(s4, -1);

        //若放白色
        chess = 2;
        s1 = s2 = s3 = s4 = 0;  //四个方向的棋子的数量
        //横向检查
        a = b = 0;   //两端连续棋子数量
        for (int k = i - 1; k >= 0; k--)
        {
            if (map[k, j] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int k = i + 1; k < 15; k++)
        {
            if (map[k, j] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s1 = a + b + 1;
        //纵向检查
        a = b = 0;
        for (int k = j - 1; k >= 0; k--)
        {
            if (map[i, k] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int k = j + 1; k < 15; k++)
        {
            if (map[i, k] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s2 = a + b + 1;
        //斜左下
        a = b = 0;
        for (int m = i - 1, n = j - 1; m >= 0 && n >= 0; m--, n--)
        {
            if (map[m, n] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int m = i + 1, n = j + 1; m < 15 && n < 15; m++, n++)
        {
            if (map[m, n] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s3 = a + b + 1;
        //斜右下
        a = b = 0;
        for (int m = i - 1, n = j + 1; m >= 0 && n < 15; m--, n++)
        {
            if (map[m, n] == chess)
            {
                a++;
            }
            else
            {
                break;
            }
        }
        for (int m = i + 1, n = j - 1; m < 15 && n >= 0; m++, n--)
        {
            if (map[m, n] == chess)
            {
                b++;
            }
            else
            {
                break;
            }
        }
        s4 = a + b + 1;
        white = GetScore(s1, 1) + GetScore(s2, 1) + GetScore(s3, 1) + GetScore(s4, 1);
    }

    public int GetScore(int x, int m)
    {
        int ans = 0;
        if (m == 1)
        {
            if (x == 1) ans += 1;
            else if (x == 2) ans += 10;
            else if (x == 3) ans += 100;
            else if (x == 4) ans += 1000;
            else ans += 10000;
        }
        else if (m == -1)
        {
            if (x == 1) ans += 1;
            else if (x == 2) ans += 8;
            else if (x == 3) ans += 64;
            else if (x == 4) ans += 512;
            else ans += 4096;
        }
        return ans;
    }

    private void GameTree(int[,] map, GameTree parent)
    {
        ScorePos[] sp = new ScorePos[225];
        //筛选前几个优良方案
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (map[i, j] == 0)
                {
                    int m = 0, n = 0;
                    GetScore(i, j, ref m, ref n, map);  //引用参数
                    sp[i * 15 + j].score = m + n;
                    sp[i * 15 + j].posX = i;
                    sp[i * 15 + j].posY = j;
                }
            }
        }
        Array.Sort(sp, comparer);
        parent.child = new GameTree[width];
        for (int i = 0; i < width; i++)
        {
            GameTree point = new GameTree();
            point.child = new GameTree[width];
            parent.child[i] = point;        //父子关系
            point.parent = parent;
            point.depth = parent.depth + 1; //博弈树深度
            point.posX = sp[i].posX;        //落子位置
            point.posY = sp[i].posY;
            point.chess = parent.chess % 2 + 1; //交替下棋

            //计算分数
            if (parent.depth % 2 == 0)
            {    //偶数层（我方）
                point.score = sp[i].score;
            }
            else
            {  //奇数层（对手）
                point.score = -sp[i].score;
            }
            if (parent.score < -6000 || parent.score > 6000)
            {
                point.score = parent.score;    //一旦一方胜利，便没有必要再算下去（防止局势扭转）
            }
            else
            {
                point.score += parent.score;//累加父层
            }

            //更新解决方案
            if (point.depth == 2)
            { 
                //节点隶属的解决方案
                point.solve = i;
            }
            else
            {
                point.solve = parent.solve;
            }

            //拓展新地图
            int[,] map1 = new int[15, 15];
            for (int j = 0; j < 15; j++)
            {
                for (int k = 0; k < 15; k++)
                {
                    map1[j, k] = map[j, k];
                }
            }
            map1[point.posX, point.posY] = point.chess;
            if (point.depth < depth)
            {
                GameTree(map1, point);
            }

            //层数上限，开始收尾
            else
            {
                int maxScore = -1;
                GameTree point1 = new GameTree();
                for (int i1 = 0; i1 < 15; i1++)
                {
                    for (int j1 = 0; j1 < 15; j1++)
                    {
                        if (map1[i1, j1] == 0)
                        {
                            int m = 0, n = 0;
                            GetScore(i1, j1, ref m, ref n, map1);  //引用参数
                            if (maxScore < m + n)
                            {
                                maxScore = m + n;
                            }
                        }
                    }
                }

                //更新point分数
                if (point.score > 6000 || point.score < -6000)
                {
                    point1.score = point.score;
                }
                else
                {
                    point1.score = point.score + maxScore;
                }
                point.score = point1.score;
                //print(point.ToString());
            }
        }
    }
    //更新树所有节点的分数
    private int UpdateTreeScore(GameTree tree)
    {
        if (tree.depth == depth)
        {    //末端
            return tree.score;
        }
        if (tree.depth % 2 == 0)
        {  //偶数层，求最小子分数
            int minScore = 100000;
            for (int i = 0; i < width; i++)
            {
                if (tree.child[i].score < minScore)
                {
                    minScore = UpdateTreeScore(tree.child[i]);
                }
            }
            return minScore;
        }
        else
        {  //奇数层，求最大子分数
            int maxScore = -100000;
            for (int i = 0; i < width; i++)
            {
                if (tree.child[i].score > maxScore)
                {
                    maxScore = UpdateTreeScore(tree.child[i]);
                }
            }
            return maxScore;
        }
    }
    #endregion

}