using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_Laser_Warning : MonoBehaviour
{
    GameObject target_player;
    // Start is called before the first frame update
    void Start()
    {
        target_player = BossStatus.instance.target_player;

    }

    IEnumerator StartRotate()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 5)
                break;

            if (target_player.transform.position.x - this.transform.position.x > 0)
                this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
            else
                this.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));

            yield return waittime;
        }
    }

}
