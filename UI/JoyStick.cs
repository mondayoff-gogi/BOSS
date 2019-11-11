using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public GameObject BackGround;
    public GameObject Handle;
    public Camera UICamera;
    private Vector2 StickPos;
    public Vector2 dir;
    private const float outline = 0.09f;
    // Start is called before the first frame update
    void Start()
    {
        if (!OptionManager.instance.Is_UseJoyStick)
        {
            this.gameObject.SetActive(false);
        }
        BackGround.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BackGround.SetActive(true);
            StickPos = UICamera.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = StickPos;
        }
        else if(Input.GetMouseButton(0))
        {
            dir = ((Vector2)UICamera.ScreenToWorldPoint(Input.mousePosition) - StickPos);

            if(dir.magnitude< outline)
            {
                Handle.transform.position = (Vector2)UICamera.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                Handle.transform.localPosition = dir.normalized* outline*240;
            }
            GameManage.instance.JoysitckDir = (Vector2)Handle.transform.position - StickPos;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            BackGround.SetActive(false);
        }
    }
}
