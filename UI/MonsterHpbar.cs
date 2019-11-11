using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHpbar : MonoBehaviour
{
    public Camera worldCam;
    public Camera uiCam;
    public Transform target;


    private UISprite[] temp;


    private void Start()
    {
        temp = this.GetComponentsInChildren<UISprite>();

    }
    // Update is called once per frame
    void Update()
    {       
        if (target)
        {
            SetPositionHUD();
            this.GetComponent<UISlider>().value = target.GetComponent<Monster>().HP / target.GetComponent<Monster>().MaxHP;
        }       
    }
    void SetPositionHUD()
    {
        //targetposition을 게임카메라의 viewPort 좌표로 변경. 
        Vector3 position = worldCam.WorldToViewportPoint(target.position);

        //해당 좌표를 uiCamera의 World좌표로 변경. 
        transform.position = uiCam.ViewportToWorldPoint(position);
        //값 정리. 
        position = transform.localPosition;
        position.x = Mathf.RoundToInt(position.x)-40;
        position.y = Mathf.RoundToInt(position.y)-85;
        position.z = 0.0f;
        transform.localPosition = position;
    }

    public void SetActive() { }
}
