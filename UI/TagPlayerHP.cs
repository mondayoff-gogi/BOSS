using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagPlayerHP : MonoBehaviour
{
    bool switchtag = false;

    public GameObject TagHP;
    public GameObject TagLabel;
    private GameObject[] player;

    public float timer;
    private const float SwitchTimer = 15f;
    // Start is called before the first frame update
    void Start()
    {
        timer = SwitchTimer;
        player = GameObject.FindGameObjectsWithTag("Player");
        this.GetComponent<UISprite>().spriteName = (UpLoadData.character_index[1]+1).ToString();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        this.GetComponent<UISprite>().fillAmount = timer / SwitchTimer;
        if(this.GetComponent<UISprite>().fillAmount>=1&& player[0].GetComponent<CharacterStat>().Tag_HP>0)
        {
            this.GetComponent<BoxCollider2D>().enabled = true;
            this.GetComponent<UISprite>().color = Color.white;
            TagHP.GetComponent<UISprite>().color = Color.green;
            TagLabel.GetComponent<UILabel>().color = Color.white;
        }
        else
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<UISprite>().color = Color.gray;
            TagHP.GetComponent<UISprite>().color = Color.gray;
            TagLabel.GetComponent<UILabel>().color = Color.gray;
        }

    }
    public void SwitchPlayerSprite()
    {
        if(switchtag)
        {
            this.GetComponent<UISprite>().spriteName = (UpLoadData.character_index[1] + 1).ToString();
            TagHP.GetComponent<UISprite>().fillAmount = player[0].GetComponent<CharacterStat>().Tag_HP / DatabaseManager.instance.Char_MaxHP[UpLoadData.character_index[1]];
        }
        else
        {
            this.GetComponent<UISprite>().spriteName = (UpLoadData.character_index[0] + 1).ToString();
            TagHP.GetComponent<UISprite>().fillAmount = player[0].GetComponent<CharacterStat>().Tag_HP / DatabaseManager.instance.Char_MaxHP[UpLoadData.character_index[0]];
        }
        switchtag = !switchtag;
        timer = 0;
    }
    
}
