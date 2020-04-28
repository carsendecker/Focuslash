using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    public bool SpawnObjectOnFinish;
    public GameObject ObjectToSpawn;
    public bool KeepDoorsClosedOnFinish;
    public bool SpawnInFirstWave;
    
    public GameObject SpawnParticlePrefab;
    public AudioClip SpawningSound;

    private int waveNumber;
    private bool spawningWave;
    private BlockerDoorScript[] roomDoors;
    private bool started;
    
    //TODO: Support for force-spawning waves? (maybe)

    private void Awake()
    {
        int startingWave = 1;

        if (SpawnInFirstWave)
        {
            startingWave = 0;
            waveNumber = -1;
        }

        for (int i = startingWave; i < EnemyWaves.Count; i++)
        {
            foreach (GameObject enemy in EnemyWaves[i].Enemies)
            {
                enemy.SetActive(false);
            }
        }

        if (ObjectToSpawn != null) ObjectToSpawn.SetActive(false);

        roomDoors = GetComponentsInChildren<BlockerDoorScript>();
    }
    

    void Update()
    {
        if (waveNumber >= EnemyWaves.Count) return;

        if (EnemyWaves[waveNumber].Enemies.Count == 0 && !spawningWave)
        {
            StartCoroutine(SpawnNextWave());
        }
        else
        {
            /*
             * This is really dumb, but in order to check if an object reference is missing, im just trying to do something random
             * with a gameobject and if it throws an error (prob NullReference), then it can remove it from the list.
             */
            try
            {
                int randomAssThing = EnemyWaves[waveNumber].Enemies[0].gameObject.layer;
            }
            catch (NullReferenceException e)
            {
                EnemyWaves[waveNumber].Enemies.RemoveAt(0);
            }
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
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f)); //*Another* delay to space out multiple spawns
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
        enemy.GetComponent<Enemy>().enabled = true;
        Destroy(particles);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player enters the room, close all doors in the room and aggro the enemies towards the player (aka enable their scripts)
        if (other.CompareTag("Player") && !started)
        {
            foreach (BlockerDoorScript door in roomDoors)
            {
                door.closeDoorWay();
            }

            foreach (var enemy in EnemyWaves[0].Enemies)
            {
                enemy.GetComponent<Enemy>().Aggro(true);
            }

            started = true;
            
            //If the first wave will spawn in, spawn em
            if(SpawnInFirstWave)
                StartCoroutine(SpawnNextWave());

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (waveNumber >= EnemyWaves.Count) return;
        
        //If something leaves the trigger, if it was an enemy, remove it from the list as it probably died
        if (EnemyWaves[waveNumber].Enemies.Contains(other.gameObject))
            EnemyWaves[waveNumber].Enemies.Remove(other.gameObject);
    }

    /// <summary>
    /// Gets called when the all enemies have been killed in a room
    /// </summary>
    private void RoomOver()
    {
        //Make all doors slashable
        if (!KeepDoorsClosedOnFinish)
        {
            foreach (BlockerDoorScript door in roomDoors)
            {
                door.makeDoorSlashable();
            }
        }

        if (SpawnObjectOnFinish)
        {
            ObjectToSpawn.SetActive(true);
        }
        
        Destroy(this);
    }

    
    
    //=================== EDITOR TOOL METHODS ==================//

#if UNITY_EDITOR
    
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
        //Using ToList() so there are no errors when removing an enemy during iteration
        foreach (Wave wave in EnemyWaves.ToList())
        {
            foreach (GameObject enemy in wave.Enemies.ToList())
            {
                //If an enemy reference is missing, remove it from the list
                try
                {
                    Handles.Label(enemy.transform.position, index.ToString(), labelStyle);
                }
                catch (Exception e)
                {
                    wave.Enemies.Remove(enemy);
                }
                
            }

            index++;
        }
    }
    
#endif

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
        if (waveNum == EnemyWaves.Count || EnemyWaves.Count == 0)
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
        
        EnemyWaves[waveNum].Enemies.Add(enemyToAssign);

        //If the wave is now empty, remove the wave
        if(EnemyWaves[removedIndex].Enemies.Count == 0) 
            EnemyWaves.RemoveAt(removedIndex);
        

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
