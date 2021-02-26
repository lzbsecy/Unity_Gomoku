using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 开启人机模式
    /// </summary>
    public void PlayPVE()
    {
        SceneManager.LoadScene("AiScenes");
    }

    /// <summary>
    /// 开启对战模式
    /// </summary>
    public void PlayPVP()
    {
        SceneManager.LoadScene("NetScenes");
    }

    /// <summary>
    /// 返回开始菜单
    /// </summary>
    public void BackMenu()
    {
        SceneManager.LoadScene("MenuScenes");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// 低级AI
    /// </summary>
    public void SelectLevelLow()
    {
        GameObject.Find("Chessboard").GetComponent<GameStatus>().ai = AI.Low;
    }

    /// <summary>
    /// 高级AI
    /// </summary>
    public void SelectLevelHigh()
    {
        GameObject.Find("Chessboard").GetComponent<GameStatus>().ai = AI.High;
    }

}
