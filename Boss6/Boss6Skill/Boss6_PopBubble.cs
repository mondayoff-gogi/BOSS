using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_PopBubble : MonoBehaviour
{
    public GameObject destroy_effect;
    public GameObject skull;

    private GameObject player;
    private CircleCollider2D _col;
    private int player_index;
    private GameObject destroy_temp;
    private GameObject skull_temp;
    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.parent.gameObject;
        player_index = player.GetComponent<CharacterStat>().char_index;
        _col = this.GetComponent<CircleCollider2D>();
        _col.enabled = true;
        skull_temp = Instantiate(skull, this.transform);
        skull_temp.transform.localScale = Vector3.one;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<CharacterStat>().char_index != player_index)
            {
                SoundManager.instance.Play(54);
                player.GetComponentInChildren<BoxCollider2D>().enabled = true;
                player.GetComponent<Character_Control>().Runable = true;
                destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
                destroy_temp.transform.localScale = Vector3.one * 4f;

                Destroy(skull_temp);
                Destroy(destroy_temp, 0.5f);
                Destroy(this.transform.parent.gameObject);
            }
        }

    }
}
