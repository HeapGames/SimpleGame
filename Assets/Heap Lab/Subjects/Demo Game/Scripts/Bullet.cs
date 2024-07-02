using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Range = 20f;
    public float Speed = 4f;
    private bool go = false;
    private Vector3 direction;
    private float deltaMove;

    public Action<Bullet> OnOutOfRange;
    public Action<Bullet> OnHit;
    private void Update()
    {
        if(go)
        {
            Vector3 translation = direction * Speed * Time.deltaTime;
            transform.Translate(translation);

            deltaMove += translation.magnitude;

            if (deltaMove >= Range)
            {
                //out of range
                OnOutOfRange?.Invoke(this);
                deltaMove = 0f;
            }
        }
    }
    public void Go(Vector3 dir)
    {
        direction = dir.normalized;
        go = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null) 
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                enemy.Hit(hitPoint);
                OnHit?.Invoke(this);
                deltaMove = 0f;
            }
        }
    }

}
