using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBuffStart : MonoBehaviour
{
    private Transform thisparent;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(15);

        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
            StartCoroutine(DelayAnimate());
        Destroy(this.gameObject, 2f);
    }

    IEnumerator DelayAnimate()
    {
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        thisparent.GetComponent<Character_Control>().Runable = false;
        yield return new WaitForSeconds(0.1f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
        yield return new WaitForSeconds(0.7f);
        thisparent.GetComponent<Character_Control>().Runable = true;
    }
}
