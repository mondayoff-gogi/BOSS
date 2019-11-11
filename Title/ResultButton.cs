using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ResultButton : MonoBehaviour
{
    private Button button;
    private int index;
    public GameObject game_manager;
    GameManage manager;

    private void Start()
    {
        button = this.GetComponent<Button>();
        manager = game_manager.GetComponent<GameManage>();
    }

    public void GetIndex()
    {
        index = Convert.ToInt32(button.name);
        manager.DisplayItemOnDescription(index);
    }
}
