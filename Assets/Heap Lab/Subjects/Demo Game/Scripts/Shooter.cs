using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public ObjectPool BulletPool;

    public Transform BulletSpawnRef;

    public float Period = 1.0f;

    public float Damage = 35f;

    public float Range = 500f;

    public bool ShootingActive = false;

    public List<Bullet> Bullets;

    float timer = 0;

    private float power;

    private void Update()
    {

        if (ShootingActive)
        {
            if (timer < Period)
            {
                timer += Time.deltaTime;
            }
            else
            {
                MoveBullet();
                timer = 0;
            }
        }
    }

    public void UpdateShootingActivity(bool val, float power)
    {
        ShootingActive = val;
        this.power = power;
    }



    private void MoveBullet()
    {
        var obj = BulletPool.GetPoolObject();
        obj.transform.position = BulletSpawnRef.position;
        obj.SetActive(true);
        //obj.transform.localScale = Vector3.one * 0.5f * power;
        Period = 0.25f / power;
        Damage = 35f * power;

        var bullet = obj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.OnOutOfRange = OnBulletOutOfRange;
            bullet.OnHit = OnBulletHit;
            bullet.Go(transform.forward, Damage);
        }
    }

    private void OnBulletHit(Bullet bullet)
    {
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }

    private void OnBulletOutOfRange(Bullet bullet)
    {
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }
}
