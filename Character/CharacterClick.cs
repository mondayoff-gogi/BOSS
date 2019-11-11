using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterClick : MonoBehaviour
{

    public bool skill_button=false;

    public BoxCollider2D _collider;
    Vector2 vec;
    private void Start()
    {
        _collider=this.GetComponent<BoxCollider2D>();
        vec = new Vector2(0.8f, 1.2f);
    }
    private void Update()
    {
        _collider.size = Camera.main.orthographicSize* vec/6;
    }
    public void Skill_button()
    {
        skill_button = true;
    }
    public bool IsThisCharacter(RaycastHit2D hit)
    {
        if(hit)
        {
            if (hit.collider.gameObject == this.gameObject&& !skill_button)
            {
                return true;
            }
            else
            {
                skill_button = false;
                return false;
            }
        }
        else
        {
            skill_button = false;
            return false;
        }

    }
    public void CharacterDead()
    {
        this.gameObject.layer = 0;
    }
    public void CharacterRevive()
    {
        this.gameObject.layer = 14;
    }

}
