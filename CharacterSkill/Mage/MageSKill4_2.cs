using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSKill4_2 : MonoBehaviour
{
    public GameObject hit_effect;

    private const float multiple = 3f;
    private GameObject player;
    private GameObject hit_temp;
    private Vector3 des;
    private Vector2 dir;
    private float speed = 8f;
    private float time;
    // Start is called before the first frame update

    private void Start()
    {
        Invoke("MakeExplosion", 2.6f);
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * speed * Time.deltaTime);
        time += Time.deltaTime;
    }

    public void GetParameter(Vector3 destination, GameObject player)
    {
        this.player = player;
        des = destination;
        dir = destination - this.transform.position;
        dir.Normalize();
    }

    private void MakeExplosion()
    {
        hit_temp = Instantiate(hit_effect, this.transform.position, Quaternion.identity);
        ///hit_temp.GetComponent<MageSkill4>().GetPlayer(player);
        Destroy(this.gameObject);
    }
}
