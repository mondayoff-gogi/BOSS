using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBoss_Boss : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = this.gameObject.GetComponent<Animator>();
        _animator.SetFloat("DirX", 0f);
        _animator.SetFloat("DirY", -1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
