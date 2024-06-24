using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator Animator;
    public float Speed = 5f;
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


}
