using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player MyPlayer;
    public ObjectPool EnemyPool;
    public List<Transform> EnemySpawnPlace;

    public Image TimerBar;
    public Image HealthBar;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;

    private float levelDuration;
    private float levelTimer;
    private float enemySpawnPeriod = 5f;
    private float spawnTimeCounter = 0f;

    private int score = 0;
    private int highScore = 0;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("high_score"))
        {
            highScore = PlayerPrefs.GetInt("high_score");
        }
        else
        {
            PlayerPrefs.SetInt("high_score", 0);
        }
    }


    private void Update()
    {
        spawnTimeCounter += Time.deltaTime;
        if(spawnTimeCounter >= enemySpawnPeriod)
        {
            spawnTimeCounter = 0;

            Transform spawnPlace = FindClosestSpawnPlace();
            GameObject enemyObj = EnemyPool.GetPoolObject();
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.SetTarget(MyPlayer.transform);
            enemy.transform.position = spawnPlace.transform.position; // + Vector3.down * 3f;
            enemyObj.SetActive(true);
            enemy.OnDied += OnEnemyKill;
        }

        HealthBar.fillAmount = MyPlayer.Health;

        levelTimer += Time.deltaTime;
        TimerBar.fillAmount = levelTimer / levelDuration;

        if(levelTimer >= levelDuration || MyPlayer.Health <= 0f)
        {
            //Level end
            if(score > highScore)
            {
                SaveProgress();
            }
        }

        Transform close_enemy = FindClosestEnemy(Shooter.Range);
        MyPlayer.Shooter.ShootingActive = close_enemy != null;

        if (close_enemy != null)
        {
            //MyPlayer.Rotate(close_enemy.transform.position);
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("high_score", score);
        PlayerPrefs.Save();
    }

    private void ShowRestartPopup()
    {

    }

    private void OnEnemyKill(Enemy enemy)
    {
        enemy.OnDied -= OnEnemyKill;
        enemy.ResetBeforePool();
        EnemyPool.ReturnPoolObject(enemy.gameObject);
    }

    private Transform FindClosestSpawnPlace()
    {
        int minDistIndex = -1;
        float MinDistance = 1000000000;

        for (int i = 0; i < EnemySpawnPlace.Count; i++)
        {
            var spawner = EnemySpawnPlace[i];
            float dist = Vector3.Distance(spawner.position, MyPlayer.transform.position);

            if(dist < MinDistance)
            {
                MinDistance = dist;
                minDistIndex = i;
            }
        }

        return EnemySpawnPlace[minDistIndex];
    }


    private Transform FindClosestEnemy(float range)
    {
        int minDistIndex = -1;
        float MinDistance = 1000000000;

        if(EnemyPool.ActivePoolObjects.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < EnemyPool.ActivePoolObjects.Count; i++)
        {
            var enemy = EnemyPool.ActivePoolObjects[i];
            float dist = Vector3.Distance(enemy.transform.position, MyPlayer.transform.position);

            if (dist < MinDistance)
            {
                MinDistance = dist;
                minDistIndex = i;
            }
        }

        if (MinDistance > range)
            return null;

        return EnemyPool.ActivePoolObjects[minDistIndex].transform;
    }


}
