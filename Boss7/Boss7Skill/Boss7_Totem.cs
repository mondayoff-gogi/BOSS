using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_Totem : MonoBehaviour
{
    public GameObject blood_pool;
    public GameObject cloud_effect;

    private GameObject cloud_temp;
    private Vector2 destination;
    private Vector2 dir;
    private Camera main;
    private const float rotate_speed = 1000f;
    private float move_speed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        destination = new Vector2(this.transform.position.x, this.transform.position.y - 51f);
        dir = destination - (Vector2)this.transform.position;
        dir.Normalize();
        main = Camera.main;
        StartCoroutine(Move());
    }



    private void SetActive()
    {
        main.GetComponent<Camera_move>().VivrateForTime(1f);
        cloud_temp = Instantiate(cloud_effect, this.transform.position, Quaternion.identity);
        Destroy(cloud_temp, 0.5f);
        this.transform.rotation = Quaternion.Euler(0f, 0f, -120f);
        blood_pool.SetActive(true);
    }

    IEnumerator Move()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);
        while (true)
        {
            this.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotate_speed));
            this.transform.Translate(dir * move_speed * Time.deltaTime, Space.World);
            if (this.transform.position.y- destination.y <= 0)
                break;
            yield return waittime;
        }
        SetActive();
        yield return 0;
    }

}
