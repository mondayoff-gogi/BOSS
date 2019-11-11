using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Spread : MonoBehaviour
{
    public float multiple = 0.5f;
    public GameObject prefab;
    public GameObject swamp;

    private GameObject swamp_temp;
    private GameObject prefab_temp;
    private Collider2D[] _collider;
    private float damage_amount;

    private ParticleSystem.MainModule setting;

    // Start is called before the first frame update
    void Start()
    {
        setting = this.GetComponent<ParticleSystem>().main;
        StartCoroutine(Explosion());
        damage_amount = BossStatus.instance.MagicAttackPower;
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(1.0f);


        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            _collider = Physics2D.OverlapCircleAll(this.transform.position, 2f);
            for (int i = 0; i < _collider.Length; i++)
            {                
                if(_collider[i])
                {
                    if (_collider[i].name == "Spread(Clone)" && _collider[i].gameObject != this.gameObject)//장판끼리 부딫히는 경우만 폭발
                    {
                        for(int j = 2; j < 4; j++)
                        {
                            if (UpLoadData.boss_usedskill_list[j][UpLoadData.boss_usedskill_list[j].Length - 1])
                                yield return 0;
                            else
                                UpLoadData.boss_usedskill_list[j][UpLoadData.boss_usedskill_list[j].Length - 1] = true;
                        }

                        setting.startColor = new Color(1, 0, 0);
                        yield return new WaitForSeconds(1.5f); //색 바뀌고

                        prefab_temp = Instantiate(prefab, this.transform); //폭발 이펙트
                        prefab_temp.transform.SetParent(this.transform.parent);
                        swamp_temp = Instantiate(swamp, this.transform);
                        swamp_temp.transform.SetParent(this.transform.parent);
                        //if(NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                        //{
                        //    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 16, swamp_temp.transform, 100, false);
                        //}
                        yield return new WaitForSeconds(0.5f);
                        Destroy(this.gameObject);
                    }
                    else if (_collider[i].CompareTag("Player"))
                    {
                        _collider[i].GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
                    }

                }
            }
                
        }          
    }
}

