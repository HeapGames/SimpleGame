using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Animator Animator;
    public float Speed = 5f;
    public Action<Enemy> OnDied;

    private ObjectPool HitParticlePool;
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

    public void SetHitParticlePool(ObjectPool hitParticlePool)
    {
        HitParticlePool = hitParticlePool;
    }

    public void Fight()
    {
        Vector3 direction = (target.position - transform.position);

        float dist = direction.magnitude;

        Vector3 unitDirection = direction.normalized;
        unitDirection.y = 0;
        transform.LookAt(transform.position + unitDirection);

        float speed = Speed;
        if(dist < 8f)
        {
            speed = Speed * 0.6f;
        }

        transform.position = transform.position + unitDirection * Time.deltaTime * Speed;
    }

    public void Hit(Vector3 hitPoint, float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            //Death
            OnDied?.Invoke(this);
            Health = 0;
        }
        else
        {
            StartCoroutine(PlayHitParticle(hitPoint));
        }

    }

    IEnumerator PlayHitParticle(Vector3 hitPoint)
    {
        var obj = HitParticlePool.GetPoolObject();
        obj.transform.position = hitPoint;

        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        HitParticlePool.ReturnPoolObject(obj);
    }


    public void ResetBeforePool()
    {
        Health = 100;
    }

}
