using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Wave
{
    public List<GameObject> Enemies;

    public Wave()
    {
        Enemies = new List<GameObject>();
    }
}

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Each list is a wave, add enemy GameObjects to a wave")]
    public List<Wave> EnemyWaves = new List<Wave>();

    [Tooltip("How long (in seconds) an enemy takes to actually instantiate after the spawn animation begins.")]
    public float SummonDelay = 2;
    
    public GameObject SpawnParticlePrefab;
    public AudioClip SpawningSound;

    private int waveNumber;
    private bool spawningWave;
    private bool doneSpawning;
    private BlockerDoorScript[] roomDoors;
    
    //TODO: Support for making last enemy drop something (or just make room drop something)
    //TODO: Support for enabling/disabling doors
    //TODO: Support for force-spawning waves? (maybe)

    private void Awake()
    {
        for (int i = 1; i < EnemyWaves.Count; i++)
        {
            foreach (GameObject enemy in EnemyWaves[i].Enemies)
            {
                // SpawnPositions.Add(enemy, enemy.transform.position);
                enemy.SetActive(false);
            }
        }

        roomDoors = GetComponentsInChildren<BlockerDoorScript>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (doneSpawning) return;
        
        if (EnemyWaves[waveNumber].Enemies.Count == 0 && !spawningWave)
        {
            StartCoroutine(SpawnNextWave());
        }
    }

    //Spawns all enemies in a wave, slightly staggered
    IEnumerator SpawnNextWave()
    {
        spawningWave = true;
        waveNumber++;

        if (waveNumber > EnemyWaves.Count - 1)
        {
            RoomOver();
            yield break;
        }

        for (int i = 0; i < EnemyWaves[waveNumber].Enemies.Count; i++)
        {
            StartCoroutine(SpawnEnemy(EnemyWaves[waveNumber].Enemies[i]));
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f)); //*Another* delay to space out multiple spawns
        }
        
        spawningWave = false;
    }

    //Actually does the spawning of enemies
    IEnumerator SpawnEnemy(GameObject enemy)
    {
        Vector2 spawnPos = Vector2.zero;
        
        spawnPos = enemy.transform.position;
        
        Services.Audio.PlaySound(SpawningSound, SourceType.AmbientSound);
        GameObject particles = Instantiate(SpawnParticlePrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(SummonDelay);

        enemy.SetActive(true);
        // EnemiesAlive.Add(Instantiate(enemy, spawnPos, Quaternion.identity));
        Destroy(particles);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (BlockerDoorScript door in roomDoors)
            {
                door.closeDoorWay();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (waveNumber >= EnemyWaves.Count) return;
        
        if (EnemyWaves[waveNumber].Enemies.Contains(other.gameObject))
            EnemyWaves[waveNumber].Enemies.Remove(other.gameObject);
    }

    private void RoomOver()
    {
        doneSpawning = true;
        foreach (BlockerDoorScript door in roomDoors)
        {
            door.makeDoorSlashable();
        }
        
        Destroy(this);
    }

    
    
    //=================== EDITOR TOOL METHODS ==================//
    
    
    /// <summary>
    /// Draws gizmos in scene view to show the wave an enemy is assigned to.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        DrawWEnemyWaveNumbers();
    }

    public void DrawWEnemyWaveNumbers()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 20;
        labelStyle.normal.textColor = Color.green;
        labelStyle.fontStyle = FontStyle.Bold;
        
        int index = 0;
        foreach (Wave wave in EnemyWaves)
        {
            foreach (GameObject enemy in wave.Enemies)
            {
                Handles.Label(enemy.transform.position, index.ToString(), labelStyle);
            }

            index++;
        }
    }
    
    /// <summary>
    /// Assigns a given enemy to a new spawning wave number.
    /// </summary>
    public void AssignToWave(GameObject enemyToAssign, int waveNumber)
    {
        int waveNum = waveNumber;
        if (waveNum > EnemyWaves.Count)
        {
            waveNum = EnemyWaves.Count;
        }

        if (waveNum == EnemyWaves.Count)
        {
            EnemyWaves.Add(new Wave());
        }

        int removedIndex = 0;
        foreach (Wave wave in EnemyWaves)
        {
            if (wave.Enemies.Contains(enemyToAssign))
            {
                removedIndex = EnemyWaves.IndexOf(wave);

                //Decrement the wave number to add the enemy to by 1 if the old wave will be empty.
                //This is due to the next wave moving down 1 place when the old one is removed.
                if (wave.Enemies.Count == 1 && waveNum > removedIndex) waveNum--;
                
                wave.Enemies.Remove(enemyToAssign);
                break;
            }
        }
        
        //If the wave is now empty, remove the wave
        if(EnemyWaves[removedIndex].Enemies.Count == 0) 
            EnemyWaves.RemoveAt(removedIndex);
        
        EnemyWaves[waveNum].Enemies.Add(enemyToAssign);

    }

    /// <summary>
    /// Returns the wave number that a given enemy is in.
    /// </summary>
    public int GetWaveNumber(GameObject enemyToFind)
    {
        int index = 0;
        foreach (Wave wave in EnemyWaves)
        {
            if(wave.Enemies.Contains(enemyToFind))
                return index;
            index++;
        }

        return -1;
    }
}
