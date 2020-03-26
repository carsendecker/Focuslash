using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Each list is a wave, add enemy GameObjects to a wave")]
    public List<List<GameObject>> EnemyWaves = new List<List<GameObject>>();
    public List<GameObject> EnemiesAlive = new List<GameObject>();
    public Dictionary<GameObject, Vector3> SpawnPositions = new Dictionary<GameObject, Vector3>();
    
    [Tooltip("How long (in seconds) an enemy takes to actually instantiate")]
    public float SummonDelay;
    
    public GameObject SpawnParticlePrefab;
    public AudioClip SpawningSound;

    private int waveNumber;

    private void Awake()
    {
        for (int i = 0; i < EnemyWaves.Count; i++)
        {
            foreach (GameObject enemy in EnemyWaves[i])
            {
                SpawnPositions.Add(enemy, enemy.transform.position);
            }
        }
    }

    void Start()
    {
        StartCoroutine(SpawnNextWave());
    }

    void Update()
    {
        if (EnemyWaves[waveNumber].Count == 0)
        {
            
        }
    }

    //Infinitely spawns enemies
    IEnumerator SpawnNextWave()
    {
        for (int i = 0; i < EnemyWaves[waveNumber].Count; i++)
        {
            StartCoroutine(SpawnEnemy(EnemyWaves[waveNumber][i]));
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f)); //*Another* delay to space out multiple spawns
        }

        waveNumber++;
    }

    //Actually does the spawning of enemies
    IEnumerator SpawnEnemy(GameObject enemy)
    {
        Vector2 spawnPos = Vector2.zero;
        Collider2D enemyCheck;
        
        spawnPos = SpawnPositions[enemy];
        
        Services.Audio.PlaySound(SpawningSound, SourceType.AmbientSound);
        GameObject particles = Instantiate(SpawnParticlePrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(SummonDelay);

        EnemiesAlive.Add(Instantiate(enemy, spawnPos, Quaternion.identity));
        Destroy(particles);
    }

}
