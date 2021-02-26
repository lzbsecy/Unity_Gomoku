using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject[] On;
    public GameObject[] Off;
    
    void Start()
    {
        
    }

    public void SwitchOn()
    {
        foreach(var obj in On)
        {
            obj.SetActive(true);
        }
    }

    public void SwitchOff()
    {
        foreach(var obj in Off)
        {
            obj.SetActive(false);
        }
    }
    
}
