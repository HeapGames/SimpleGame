public class GameConfig
{
    public float PlayerRunSpeedFactor;
    public float PlayerRotationSpeedFactor;
    public float ShootingSpeedFactor;
    public float BulletSpeedFactor;
    public float EnemyRunSpeedFactor;
    public float PlayerPowerFactor;
    public float EnemyPowerFactor;
    public float EnemySpawnPeriodFactor;

    public GameConfig(float playerRunSpeedFactor, float playerRotationSpeedFactor, float shootingSpeedFactor, float bulletSpeedFactor, float enemyRunSpeedFactor, float playerPowerFactor, float enemyPowerFactor, float enemySpawnPeriod)
    {
        PlayerRunSpeedFactor = playerRunSpeedFactor;
        PlayerRotationSpeedFactor = playerRotationSpeedFactor;
        ShootingSpeedFactor = shootingSpeedFactor;
        BulletSpeedFactor = bulletSpeedFactor;
        EnemyRunSpeedFactor = enemyRunSpeedFactor;
        PlayerPowerFactor = playerPowerFactor;
        EnemyPowerFactor = enemyPowerFactor;
        EnemySpawnPeriodFactor = enemySpawnPeriod;
    }
}

public static class GameConfigService
{
    public static GameConfig StandardConfig 
    { 
        get 
        { 
            return new GameConfig(1f,1f,1f,1f,1f,1f,1f, 1f);
        } 
    }
    public static GameConfig UltiConfig 
    {
        get
        {
            return new GameConfig(1.7f, 0f, 2.5f, 3f, 1.2f, 5f, 2f, 0.12f);
        }
    }

    
}
