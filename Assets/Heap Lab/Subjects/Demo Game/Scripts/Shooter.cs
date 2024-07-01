using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public ObjectPool BulletPool;

    public Transform BulletSpawnRef;

    public static float Period = 1.0f;

    public static float Damage = 30f;

    public static float Range = 500f;

    public bool ShootingActive = false;

    public List<Bullet> Bullets;

    float timer = 0;

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



    private void MoveBullet()
    {
        var obj = BulletPool.GetPoolObject();
        obj.transform.position = BulletSpawnRef.position;
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one * Player.Power;
        Period = 0.1f / Player.Power;

        var bullet = obj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.OnOutOfRange = OnBulletOutOfRange;
            bullet.OnHit = OnBulletHit;
            bullet.Go(transform.forward);
        }
    }

    private void OnBulletHit(Bullet bullet)
    {
        //bullet.OnHit -= OnBulletHit;
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }

    private void OnBulletOutOfRange(Bullet bullet)
    {
        //bullet.OnOutOfRange -= OnBulletOutOfRange;
        Bullets.Remove(bullet);
        BulletPool.ReturnPoolObject(bullet.gameObject);
    }
}
