using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateSelection : MonoBehaviour
{
    public Image[] Images;
    public float radius;
    private float x, y = 0, z;
    private float angle = 270f;
    private Vector3[] Images_position;
    private float[] angle_array;
    private Vector3 dir;
    private float temp = 0;

    private bool isInputBlocked = false;
    private Vector2 screenSize;
    private float min_Swipe_Dist;
    private Vector2 swipe_Direction;
    private Vector2 touchDownPos;
    private Vector3 mousePos;
    int index = 0;
    //private bool flag = false;
    private void Awake()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        min_Swipe_Dist = 5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        radius = 3f;
        Images_position = new Vector3[Images.Length];
        angle_array = new float[Images.Length];
        for (int i = 0; i < 8; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius * 0.5f;
            z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            angle_array[i] = angle;
            angle += (360f / 8f);
            Images[i].gameObject.transform.position = new Vector3(this.transform.position.x + x, this.transform.position.y + y, this.transform.position.z + z);
            Images_position[i] = Images[i].gameObject.transform.position;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //#if UNITY_ANDROID
        //    GetMobileInput();
        //#else
        //    GetInput();
        //#endif
        GetInput();
        for(int i = 0; i < Images.Length; i++)
        {
            if(i == index)
            {
                Images[i].gameObject.transform.localScale = Vector3.one * 3f;
                continue;
            }
            Images[i].gameObject.transform.localScale = Vector3.one;
        }

    }


    //private void GetMobileInput()
    //{
    //    if(Input.touches.Length > 0)
    //    {
    //        Touch t = Input.GetTouch(0);
    //        if(t.phase == TouchPhase.Began)
    //        {
    //            touchDownPos = new Vector2(t.position.x, t.position.y);
    //        }
    //        else if(t.phase == TouchPhase.Moved)
    //        {
    //            Vector2 current_position = new Vector2(t.position.x, t.position.y);
    //            bool swipeDetected = checkSwipe(touchDownPos, current_position);
    //            swipe_Direction = (Input.mousePosition - mousePos).normalized;
    //            if (swipeDetected)
    //            {
    //                RotateImage(swipe_Direction);
    //            }
    //            else if(t.phase == TouchPhase.Ended)
    //            {

    //            }
    //        }
    //    }
    //}

    private void GetInput()
    {
        if(Input.GetMouseButtonDown(0) == true)
        {
            mousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) == true)
        {

            bool swipeDetected = checkSwipe(mousePos, Input.mousePosition);
            swipe_Direction = (Input.mousePosition - mousePos).normalized;

            if (swipeDetected)
            {
                if (swipe_Direction.x >= 0)
                {
                    temp += Time.deltaTime * 10f;
                    if (temp >= 45f)
                    {
                        //flag = true;
                        index--;
                        temp = 0;

                        if (index < 0)
                        {
                            temp = 0;
                            index = 7;
                        }
                    }
                }
                else
                {
                    temp -= Time.deltaTime * 10f;
                    if (temp <= -45f)
                    {
                        index++;
                        temp = 0;

                        if (index > 8)
                        {
                            temp = 0;
                            index = 0;
                        }
                    }

                }
                for (int i = 0; i < Images.Length; i++)
                { 
                    x = Mathf.Cos(Mathf.Deg2Rad * (temp + angle_array[i])) * radius;
                    y = Mathf.Sin(Mathf.Deg2Rad * (temp + angle_array[i])) * radius * 0.5f;
                    z = Mathf.Sin(Mathf.Deg2Rad * (temp + angle_array[i])) * radius;
                    Images[i].gameObject.transform.position = new Vector3(x, y, z);
                }

            }
            else if (Input.GetMouseButtonUp(0) == true)
            {
                temp = 0;
            }
        }

    }


    private bool checkSwipe(Vector3 downPos, Vector3 currentPos)
    {
        if (isInputBlocked == true)
        {
            return false;
        }

        Vector2 currentSwipe = currentPos - downPos;
        if (currentSwipe.magnitude >= min_Swipe_Dist)
        {
            return true;
        }

        return false;
    }
}