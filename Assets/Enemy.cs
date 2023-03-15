using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;

    public int maxHealth = 100;
    public int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int attackDamage)
    {
        anim.SetTrigger("isDamaged");
        
        currentHealth -= attackDamage;
        

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("isDead");

        GetComponent<Collider2D>().enabled = false;
    }

}
