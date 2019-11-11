using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_Bubble : MonoBehaviour
{
    public GameObject destroy_effect;
    public int player_index = 0;

    private Rigidbody2D _rig;
    private CircleCollider2D _collider;
    private GameObject _child;
    private bool is_trapped = false;
    private const float limit_time = 5f;
    private float count_time = 0;
    private GameObject player;
    private GameObject destroy_temp;
    private float running_time;
    private float y;
    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponent<CircleCollider2D>();
        _child = this.GetComponentInChildren<Boss6_PopBubble>().gameObject;
        _rig = this.GetComponent<Rigidbody2D>();
        SoundManager.instance.Play(53);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_trapped)
        {
            if (player.CompareTag("DeadPlayer"))
            {
                SoundManager.instance.Play(54);
                is_trapped = false;
            }
            count_time += Time.deltaTime;
            if ((count_time % 1) == 0)
                SoundManager.instance.Play(54);
            if(count_time >= limit_time)
            {
                destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
                destroy_temp.transform.localScale = new Vector2(4, 4);
                Destroy(destroy_temp, 0.5f);
                player.GetComponent<CharacterStat>().GetDamage(99999, false);
                //player.GetComponentInChildren<BoxCollider2D>().enabled = true;
                player.gameObject.GetComponent<Character_Control>().Runable = true;

                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("OtherPlayer"))
        {
            if(collision.gameObject.GetComponentInChildren<Boss6_PopBubble>() == null)
            {
                this.transform.SetParent(collision.transform);
                //collision.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
                collision.gameObject.GetComponent<Character_Control>().Runable = false;
                collision.gameObject.GetComponent<Character_Control>().RunningStop();
                player = collision.gameObject;
                _rig.velocity = Vector2.zero;
                _rig.angularVelocity = 0f;
                this.transform.localPosition = Vector2.zero;
                _collider.enabled = false;
                player_index = collision.gameObject.GetComponent<CharacterStat>().char_index;

                _child.GetComponent<Boss6_PopBubble>().enabled = true;
                StartCoroutine(ChangeColor());
                is_trapped = true;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

    }

    IEnumerator ChangeColor()
    {
        float t = 1;
        while(t >= 0)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(t, t, t, 1f);
            t -= 0.01f;
            yield return new WaitForSeconds(0.005f);
        }
        yield return 0;
    }


}
