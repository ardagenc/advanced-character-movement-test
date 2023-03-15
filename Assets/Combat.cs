using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    
    public int attackDamage;
    


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        
        if (Input.GetButtonDown("Attack1"))
        {
            anim.SetTrigger("Attack1");

        }

    } 



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }
}
