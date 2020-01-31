using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> EnemyTypes = new List<GameObject>();
    public float SpawnDelay; //How often enemies spawn (LARGER = SLOWER)
    public float SpawnSpeedVariance;
    public float SummonDelay; //How long an enemy takes to actually instantiate
    public float SpawnDensity; //Radius of how close an enemy can spawn next to another enemy
    public float DifficultyScaleDelay; //How fast difficulty speeds up (LARGER = SLOWER)
    public GameObject SpawnParticlePrefab;
    public AudioClip SpawningSound;

    [HideInInspector] public float MAP_WIDTH, MAP_HEIGHT;

    private float currentDifficulty = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update()
    {
        currentDifficulty += Time.deltaTime;
        SystemsManager.UI.ScoreText.text = Mathf.RoundToInt(currentDifficulty * 10).ToString();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float randVar = Random.Range(-SpawnSpeedVariance, SpawnSpeedVariance);
            float spawnTimer = (SpawnDelay + randVar) - (currentDifficulty / DifficultyScaleDelay);
            
            yield return new WaitForSeconds(spawnTimer);

            int spawnCount = 1;
            int randEnemy = Random.Range(0, EnemyTypes.Count);
            if (randEnemy == 0)
                spawnCount = Random.Range(2, 4);

            for (int i = 0; i < spawnCount; i++)
            {
                StartCoroutine(SpawnEnemy(randEnemy));
                yield return new WaitForSeconds(Random.Range(0.2f, 0.5f)); //*Another* delay to space out multiple spawns
            }
            
            //Wait for all the enemies to finish spawning
            yield return new WaitForSeconds(SummonDelay);
        }
    }

    IEnumerator SpawnEnemy(int enemy)
    {
        Vector2 spawnPos = Vector2.zero;
        Collider2D enemyCheck;
        do
        {
            float randPosX = Random.Range(4, MAP_WIDTH - 4);
            float randPosY = Random.Range(4, MAP_HEIGHT - 4);
            spawnPos = transform.TransformPoint(new Vector2(randPosX, randPosY));

            enemyCheck = Physics2D.OverlapCircle(spawnPos, SpawnDensity - (currentDifficulty / DifficultyScaleDelay));
        } while (enemyCheck != null && enemyCheck.gameObject.CompareTag("Enemy"));

        SystemsManager.Audio.PlaySound(SpawningSound, SourceType.EnemySound);
        GameObject particles = Instantiate(SpawnParticlePrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(SummonDelay);

        Instantiate(EnemyTypes[enemy], spawnPos, Quaternion.identity);
        Destroy(particles);

    }

}
