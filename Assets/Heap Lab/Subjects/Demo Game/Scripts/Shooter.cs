using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public ObjectPool BulletPool;

    public Transform BulletSpawnRef;

    public float Period = 1.0f;

    public static float Damage = 30f;

    public bool ShootingActive = true;

    public List<Bullet> Bullets;

    private void Start()
    {
        StartCoroutine(Shoot());
    }


    private IEnumerator Shoot()
    {
        float timer = 0;
        while (ShootingActive)
        {
            while(timer < Period)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            MoveBullet();

            timer = 0;
        }
    }

    private void MoveBullet()
    {
        var obj = BulletPool.GetPoolObject();
        obj.transform.position = BulletSpawnRef.position;
        obj.SetActive(true);
        var bullet = obj.GetComponent<Bullet>();


        if (bullet != null)
        {
            bullet.OnOutOfRange += OnBulletOutOfRange;
            bullet.OnHit += OnBulletHit;
            bullet.Go(transform.forward);
        }
    }

    private void OnBulletHit(Bullet bullet)
    {
        bullet.OnHit -= OnBulletHit;
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }

    private void OnBulletOutOfRange(Bullet bullet)
    {
        bullet.OnOutOfRange -= OnBulletOutOfRange;
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }
}
