using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrace : MonoBehaviour
{
    public Camera worldCam;
    public Camera uiCam;
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        if (GameManage.instance.IsGameEnd)
        {
            this.gameObject.SetActive(false);
        }
        if (target)
        {
            SetPositionHUD();
        }

        //if (target.tag == "Player")
        //{
        //    for (int i = 0; i < temp.Length; i++)
        //    {
        //        temp[i].enabled = true;
        //    }
        //}

        //if (target.tag == "DeadPlayer")
        //{
        //    for (int i = 0; i < temp.Length; i++)
        //    {
        //        temp[i].enabled = false;
        //    }
        //}
    }
    void SetPositionHUD()
    {
        //targetposition을 게임카메라의 viewPort 좌표로 변경. 
        Vector3 position = worldCam.WorldToViewportPoint(target.position);

        //해당 좌표를 uiCamera의 World좌표로 변경. 
        transform.position = uiCam.ViewportToWorldPoint(position);
        //값 정리. 
        position = transform.localPosition;
        position.x = Mathf.RoundToInt(position.x);
        position.y = Mathf.RoundToInt(position.y);
        position.z = 0.0f;
        transform.localPosition = position*worldCam.orthographicSize;
    }
}
