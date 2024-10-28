using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Obsolete("Replaced by the Spawn Manager")]
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroup;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;

    }

    public List<Wave> waves;
    public int currentWaveCount;



    [Header("Spawner Attributes")]
    float spawnTimer;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;
    bool isWaveActive = false;



    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;


    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount==0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer>=waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        yield return new WaitForSeconds(waveInterval);

        if (currentWaveCount<waves.Count-1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }
     
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGRoup in waves[currentWaveQuota].enemyGroup)
        {
            currentWaveQuota += enemyGRoup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        
    }

    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount< waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroup)
            {
                if (enemyGroup.spawnCount<enemyGroup.enemyCount)
                {
                    

                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position,Quaternion.identity);
        
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                }
            }
        }

        
    }

    public void OnEnemiesKilled()
    {
        enemiesAlive--;
       
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }



}
