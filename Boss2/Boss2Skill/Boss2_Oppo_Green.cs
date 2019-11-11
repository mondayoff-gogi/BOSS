using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Oppo_Green : MonoBehaviour
{
    Collider2D[] _collider;
    GameObject[] player;
    float timer;


    public GameObject Barrier;

    // Start is called before the first frame update
    void Start()
    {
        Barrier.SetActive(false);
        player = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(MakeInvincible());
        timer = 0;
        Destroy(this.gameObject, 3f);
    }


    IEnumerator MakeInvincible()
    {
        while(true)
        {            
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 1f);
            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Player")
                {
                    _collider[i].GetComponent<CharacterStat>().is_Invincible = true;
                    Barrier.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
            if (timer >= 3)
                break;
        }
        
    }    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().is_Invincible = false;
            Barrier.SetActive(false);
        }
    }
}
