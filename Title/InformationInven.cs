using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class InformationInven : MonoBehaviour
{
    public GameObject destroy_effect;

    private GameObject destroy_effect_temp;
    private GameObject new_item_icon;
    private Button button;
    private int item_index;
    private GameObject information;
    private int item_type;
    private Image item_portrait;
    private Image item_icon;
    private Text item_name;
    private Text item_des;

    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<Button>();
        information = GameObject.Find("Dictionary");
        item_name = information.GetComponent<Information>().item_name;
        item_portrait = information.GetComponent<Information>().item_image;
        item_des = information.GetComponent<Information>().item_description;
        item_type = information.GetComponent<Information>().GetItemType();
        item_icon = button.transform.GetChild(0).GetComponent<Image>();
        new_item_icon = information.GetComponent<Information>().new_item_icon;

    }

    public void ItemButtonClick()
    {
        SoundManager.instance.Play(38);

        item_index = Convert.ToInt32(button.name);
        information.GetComponent<Information>().GetItemIndex(item_index, this.gameObject);
        switch (item_type)
        {
            case 0:
                if (UpLoadData.item_is_gained[item_index] && !UpLoadData.item_info_list[item_index])
                {
                    UpLoadData.item_info_list[item_index] = true;
                    StartCoroutine(UnLockEquipItemEffect(10000 + item_index));
                    UpLoadData.new_item_count--;
                    ActivateNewItemIcon();
                    ResetItemIcon();
                    information.GetComponent<Information>().SetActiveNewItemIcon(0);
                }
                else
                    return;
                break;
            case 1:
                if (UpLoadData.item_is_gained[item_index + 100] && !UpLoadData.item_info_list[item_index + 100])
                {
                    UpLoadData.item_info_list[item_index + 100] = true;
                    StartCoroutine(UnLockEquipItemEffect(20000 + item_index));
                    UpLoadData.new_item_count--;
                    ActivateNewItemIcon();
                    ResetItemIcon();
                    information.GetComponent<Information>().SetActiveNewItemIcon(1);
                }
                else
                    return;
                break;
            case 2:
                if (UpLoadData.item_is_gained[item_index + 200] && !UpLoadData.item_info_list[item_index + 200])
                {
                    UpLoadData.item_info_list[item_index + 200] = true;
                    StartCoroutine(UnLockEquipItemEffect(30000 + item_index));
                    UpLoadData.new_item_count--;
                    ActivateNewItemIcon();
                    ResetItemIcon();
                    information.GetComponent<Information>().SetActiveNewItemIcon(2);
                }
                else
                    return;
                break;
        }
    }


    IEnumerator UnLockEquipItemEffect(int type)
    {
        WaitForSeconds wait_time = new WaitForSeconds(0.03f);
        //float t = 1.5f;
        Color item_icon_color = item_icon.color;
        Color color_item_name = item_name.color;
        Color color_item_des = item_des.color;
        Color color_item_image = item_portrait.color;
        destroy_effect_temp = Instantiate(destroy_effect, this.gameObject.transform.position, Quaternion.identity);
        while (item_icon_color.a > 0f)
        {
            item_icon_color.a -= 0.03f;
            item_icon.color = item_icon_color;
            yield return wait_time;
        }
        item_icon.sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == type + 1).itemIcon;
        item_icon.material = null;
        while (color_item_name.a < 1f)
        {
            item_icon_color.a += 0.03f;
            item_icon.color = item_icon_color;
            yield return wait_time;
        }
        item_icon.color = Color.white;
        yield return 0;
    }

    private void ResetItemIcon()
    {
        Color item_icon_color = item_icon.color;
        item_icon_color.a = 1f;
        item_icon.color = Color.white;
    }

    private void ActivateNewItemIcon()
    {
        if (UpLoadData.new_item_count >= 1)
        {
            new_item_icon.SetActive(true);
        }
        else
        {
            new_item_icon.SetActive(false);
        }
    }

}
