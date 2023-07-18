using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestDamageObj : MonoBehaviour, IDamageable
{
    public int health => myhealth;

    public int myhealth = 1;

    public void TakeDamage(int damage)
    {
        myhealth -= damage;
        if(myhealth <= 0)
        {
            //Debug.Log("Hit");
            Destroy(this.gameObject);
        }
    }

}
