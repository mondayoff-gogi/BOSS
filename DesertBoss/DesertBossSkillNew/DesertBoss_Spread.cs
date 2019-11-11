using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBoss_Spread : MonoBehaviour
{
    public GameObject orb;
    public GameObject effect;

    private GameObject effect_temp;
    private GameObject orb_temp;
    private int temp = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateOrb());
    }

    IEnumerator GenerateOrb()
    {
        while(temp < 3)
        {
            SoundManager.instance.Play(43);
            orb_temp = Instantiate(orb, this.transform.position, Quaternion.identity);
            effect_temp = Instantiate(effect, this.transform.parent);
            Destroy(effect_temp, 0.5f);
            temp++;
            yield return new WaitForSeconds(2f);
        }
        Destroy(this.gameObject);

    }

}
