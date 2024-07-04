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
    public Light Light;
    public ParticleSystem PlayerUltiParticle;

    private float enemySpawnPeriod = 1.5f;
    private float spawnTimeCounter = 0f;

    private int score = 0;
    private int highScore = 0;
    private int killedEnemyCounter;
    private int totalKilledEnemy;

    private GameConfig gameConfig;
    private bool IsFirstGame = true;
    public static bool IsUltiMode = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (PlayerPrefs.HasKey("high_score"))
        {
            highScore = PlayerPrefs.GetInt("high_score");
            IsFirstGame = false;
        }
        else
        {
            PlayerPrefs.SetInt("high_score", 0);
        }

        HighScoreText.text = highScore.ToString();
        gameConfig = GameConfigService.StandardConfig;
        ApplyGameConfig();
    }

    private void ResetParameters()
    {
        enemySpawnPeriod = 1.5f;
        MyPlayer.ResetParams();
    }

    private void ApplyGameConfig()
    {
        ResetParameters();

        if (gameConfig != null)
        {
            enemySpawnPeriod *= gameConfig.EnemySpawnPeriodFactor;
            MyPlayer.RunSpeed *= gameConfig.PlayerRunSpeedFactor;
            MyPlayer.Power *= gameConfig.PlayerPowerFactor;
        }
    }

    IEnumerator UltiMode()
    {
        IsUltiMode = true;
        PlayerUltiParticle.Play();
        Light.color = new Color(1f,0.79f,0.79f,1f);
        Light.intensity = 1.1f;
        gameConfig = GameConfigService.UltiConfig;
        ApplyGameConfig();
        while(true)
        {
            UltiBar.fillAmount -= Time.deltaTime * 0.05f;
            if(UltiBar.fillAmount <= 0f)
            {
                break;
            }
            yield return null;
        }

        gameConfig = GameConfigService.StandardConfig;
        ApplyGameConfig();
        IsUltiMode = false;
        killedEnemyCounter = 0;
        Light.color = Color.white;
        Light.intensity = 1f;
        PlayerUltiParticle.Stop();

    }


    private void Update()
    {
        spawnTimeCounter += Time.deltaTime;
        if(spawnTimeCounter >= enemySpawnPeriod && EnemyPool.PoolObjects.Count > 0)
        {
            spawnTimeCounter = 0;

            GameObject enemyObj = EnemyPool.GetPoolObject();
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.SetTarget(MyPlayer.transform);
            enemy.SetHitParticlePool(HitParticlePool);

            float a = Random.Range(0, 2 * Mathf.PI);

            // Bu açýya göre spawn noktasýný hesapla
            Vector3 spawnPosition = MyPlayer.transform.position + new Vector3(Mathf.Cos(a),0f, Mathf.Sin(a)) * 16f;
            spawnPosition.y = 0f;
            enemy.transform.position = spawnPosition;
            //enemy.transform.position = spawnPlace.transform.position; // + Vector3.down * 3f;
            enemyObj.SetActive(true);
            enemy.OnDied += OnEnemyKill;
        }



        Transform close_enemy = FindClosestEnemy(MyPlayer.Shooter.Range);
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
        MyPlayer.Shooter.UpdateShootingActivity(close_enemy != null & !MyPlayer.IsJumping, MyPlayer.Power);

        UpdateUI();

        if (score > highScore)
        {
            HighScoreText.text = highScore.ToString();

            if (!IsFirstGame) 
            { 
                //HighestScoreParticle.SetActive(true);
            }
        }

        if (MyPlayer.Health <= 0f)
        {
            //Level end
            Time.timeScale = 0f;
            ShowRestartPopup();

            if (score > highScore)
            {
                SaveProgress();
            }
        }


        if (killedEnemyCounter >= 5 && !IsUltiMode)
        {
            //Ulti mode
            StartCoroutine(UltiMode());
            killedEnemyCounter = 0;
        }
    }

    private void UpdateUI()
    {
        ScoreText.text = score.ToString();
        SpeedText.text = MyPlayer.RunSpeed.ToString("F1");
        PowerText.text = MyPlayer.Power.ToString("F1");
        HealthBar.fillAmount = MyPlayer.Health / 100f;

        if (!IsUltiMode)
        {
            UltiBar.fillAmount = killedEnemyCounter / 5f;
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
        killedEnemyCounter++;
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

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

}
