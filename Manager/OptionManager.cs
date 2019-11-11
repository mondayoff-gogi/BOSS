using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    static public OptionManager instance;

    public Slider slider;
    public Image JoyStickCheck;
    public bool Is_UseJoyStick;

    public Image Left;
    public Image Middle;
    public Image Right;

    public int SkillPos;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----

    // Start is called before the first frame update
    void Start()
    {
        slider.value = DatabaseManager.instance.VolumeSettingValue;
        Is_UseJoyStick = DatabaseManager.instance.JoyStickUse;
        SkillPos = DatabaseManager.instance.skillpos;
        BGMManager.instance.SetVolume(slider.value);

        if(Is_UseJoyStick)
            JoyStickCheck.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
        else
            JoyStickCheck.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;



        switch (SkillPos)
        {
            case 0:
                Left.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
                Middle.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                Right.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                break;
            case 1:
                Left.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                Middle.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
                Right.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                break;
            case 2:
                Left.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                Middle.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
                Right.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
                break;

        }
    }
    public void SetVolumn()
    {
        BGMManager.instance.SetVolume(slider.value);
        DatabaseManager.instance.VolumeSettingValue = slider.value;
    }
    public void SetJoyStick()
    {
        Is_UseJoyStick = !Is_UseJoyStick;
        DatabaseManager.instance.JoyStickUse = Is_UseJoyStick;

        if(Is_UseJoyStick)
        {
            JoyStickCheck.sprite = Resources.Load("Image/CheckOn",typeof(Sprite)) as Sprite;
        }
        else
        {
            JoyStickCheck.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
        }
    }
    public void SetLeft()
    {
        SkillPos = 0;
        DatabaseManager.instance.skillpos = SkillPos;


        Left.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
        Middle.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
        Right.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
    }
    public void SetMiddle()
    {
        SkillPos = 1;
        DatabaseManager.instance.skillpos = SkillPos;

        Left.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
        Middle.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
        Right.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
    }
    public void SetRight()
    {
        SkillPos = 2;
        DatabaseManager.instance.skillpos = SkillPos;

        Left.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
        Middle.sprite = Resources.Load("Image/CheckOff", typeof(Sprite)) as Sprite;
        Right.sprite = Resources.Load("Image/CheckOn", typeof(Sprite)) as Sprite;
    }
}
