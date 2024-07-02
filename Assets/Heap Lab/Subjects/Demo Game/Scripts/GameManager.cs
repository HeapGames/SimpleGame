using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    public Player MyPlayer;
    public ObjectPool EnemyPool;
    public ObjectPool CollectablePool;
    public ObjectPool HitParticlePool;
    public ObjectPool DestroyParticlePool;
    public GameObject HighestScoreParticle;

    public List<Transform> EnemySpawnPlace;

    public Image UltiBar;
    public Image HealthBar;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI PowerText;
    public GameObject ReplayPanel;

    private float enemySpawnPeriod = 2f;
    private float spawnTimeCounter = 0f;

    private int score = 0;
    private int highScore = 0;
    private int totalKilledEnemy;

    private GameConfig gameConfig;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (PlayerPrefs.HasKey("high_score"))
        {
            highScore = PlayerPrefs.GetInt("high_score");
        }
        else
        {
            PlayerPrefs.SetInt("high_score", 0);
        }

        HighScoreText.text = highScore.ToString();
        gameConfig = GameConfigService.StandardConfig;
    }

    private void ApplyGameConfig()
    {
        if (gameConfig == null)
        {
            enemySpawnPeriod *= gameConfig.EnemySpawnPeriodFactor;
            MyPlayer.RunSpeed *= gameConfig.PlayerRunSpeedFactor;
            MyPlayer.RotationSpeed *= gameConfig.PlayerRotationSpeedFactor;
            Player.Power *= gameConfig.PlayerPowerFactor;
            Shooter.Damage *= gameConfig.PlayerPowerFactor;
        }
    }


    private void Update()
    {
        spawnTimeCounter += Time.deltaTime;
        if(spawnTimeCounter >= enemySpawnPeriod && EnemyPool.PoolObjects.Count > 0)
        {
            spawnTimeCounter = 0;

            Transform spawnPlace = FindClosestSpawnPlace();
            GameObject enemyObj = EnemyPool.GetPoolObject();
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.SetTarget(MyPlayer.transform);
            enemy.SetHitParticlePool(HitParticlePool);

            float a = Random.Range(0, 2 * Mathf.PI);

            // Bu açýya göre spawn noktasýný hesapla
            Vector3 spawnPosition = MyPlayer.transform.position + new Vector3(Mathf.Cos(a),0f, Mathf.Sin(a)) * 20f;
            spawnPosition.y = 0f;
            enemy.transform.position = spawnPosition;
            //enemy.transform.position = spawnPlace.transform.position; // + Vector3.down * 3f;
            enemyObj.SetActive(true);
            enemy.OnDied += OnEnemyKill;
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

        if (MyPlayer.Health <= 0f)
        {
            //Level end
            if (score > highScore)
            {
                SaveProgress();
                HighScoreText.text = highScore.ToString();
            }

            Time.timeScale = 0f;
            ShowRestartPopup();
        }
    }

    private void UpdateUI()
    {
        ScoreText.text = score.ToString();
        SpeedText.text = MyPlayer.RunSpeed.ToString("F1");
        PowerText.text = Shooter.Damage.ToString("F1");
        UltiBar.fillAmount = totalKilledEnemy / 10f;
        HealthBar.fillAmount = MyPlayer.Health / 100f;

        if (UltiBar.fillAmount >= 1f)
        {
            //Ulti mode
            gameConfig = GameConfigService.UltiConfig;
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("high_score", score);
        PlayerPrefs.Save();
    }

    private void ShowRestartPopup()
    {
        ReplayPanel.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(0);
    }


    private void OnEnemyKill(Enemy enemy)
    {
        enemy.OnDied -= OnEnemyKill;
        enemy.ResetBeforePool();
        EnemyPool.ReturnPoolObject(enemy.gameObject);
        totalKilledEnemy++;
        score = totalKilledEnemy * 10;

        if (Random.Range(0, 100) < 15)
        {
            var collectable = CollectablePool.GetPoolObject();
            collectable.transform.position = enemy.transform.position + new Vector3(-5,0,-3);
            int rand_type = Random.Range(0, 3);
            collectable.GetComponent<Collectable>().SetType((CollectableType)rand_type);
            collectable.gameObject.SetActive(true);
        }
        StartCoroutine(PlayDestroyParticle(enemy.transform.position));
    }

    IEnumerator PlayDestroyParticle(Vector3 pos)
    {
        var obj = DestroyParticlePool.GetPoolObject();
        obj.transform.position = pos + Vector3.up;
        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        DestroyParticlePool.ReturnPoolObject(obj);
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
