using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator Animator;
    public float Speed = 5f;
    public Action<Enemy> OnDied;
    
    private float Health = 100f;
    private Transform target;


    private void Update()
    {
        Fight();
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void Fight()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.LookAt(transform.position + direction);
        transform.position = transform.position + direction * Time.deltaTime * Speed;
    }

    public void Hit()
    {
        Health -= Shooter.Damage;

        if (Health <= 0)
        {
            //Death
            OnDied?.Invoke(this);
            Health = 0;
        }
    }

    public void ResetBeforePool()
    {
        Health = 100;
    }

}
