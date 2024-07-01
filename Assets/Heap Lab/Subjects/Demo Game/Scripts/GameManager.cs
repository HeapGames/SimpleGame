using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Player MyPlayer;
    public ObjectPool EnemyPool;
    public ObjectPool CollectablePool;
    public List<Transform> EnemySpawnPlace;

    public Image UltiBar;
    public Image HealthBar;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI PowerText;

    private float levelDuration;
    private float levelTimer;
    private float enemySpawnPeriod = 0.75f;
    private float spawnTimeCounter = 0f;

    private int score = 0;
    private int highScore = 0;
    private int totalKilledEnemy;

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

        HighScoreText.text = highScore.ToString();
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

            float a = Random.Range(0, 2 * Mathf.PI);

            // Bu açýya göre spawn noktasýný hesapla
            Vector3 spawnPosition = MyPlayer.transform.position + new Vector3(Mathf.Cos(a),0f, Mathf.Sin(a)) * 20f;
            spawnPosition.y = 0f;
            enemy.transform.position = spawnPosition;
            //enemy.transform.position = spawnPlace.transform.position; // + Vector3.down * 3f;
            enemyObj.SetActive(true);
            enemy.OnDied += OnEnemyKill;
        }

        HealthBar.fillAmount = MyPlayer.Health;

        if(MyPlayer.Health <= 0f)
        {
            //Level end
            if(score > highScore)
            {
                SaveProgress();
                HighScoreText.text = highScore.ToString();
            }
        }

        Transform close_enemy = FindClosestEnemy(Shooter.Range);
        float angle = -1;

        if (close_enemy != null)
        {
            angle = Vector3.Angle(MyPlayer.transform.forward, -close_enemy.transform.forward);

            if (Mathf.Abs(angle) < 500f)
            {
                MyPlayer.Rotate(close_enemy.position);
            }
        }
        Debug.Log("angle : " + angle);
        MyPlayer.Shooter.ShootingActive = close_enemy != null & !MyPlayer.IsJumping;

        UpdateUI();
    }

    private void UpdateUI()
    {
        ScoreText.text = score.ToString();
        SpeedText.text = MyPlayer.RunSpeed.ToString("F1");
        PowerText.text = (Shooter.Damage / 300).ToString("F1");
        UltiBar.fillAmount = totalKilledEnemy / 10;
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
        totalKilledEnemy++;
        score = totalKilledEnemy * 10;

        if (Random.Range(0, 100) < 10)
        {
            var collectable = CollectablePool.GetPoolObject();
            collectable.transform.position = enemy.transform.position;
            collectable.gameObject.SetActive(true);
        }
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
