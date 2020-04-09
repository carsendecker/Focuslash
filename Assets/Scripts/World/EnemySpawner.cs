﻿using System;
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
    
    public GameObject SpawnParticlePrefab;
    public AudioClip SpawningSound;

    private int waveNumber;
    private bool spawningWave;
    private BlockerDoorScript[] roomDoors;
    
    //TODO: Support for making last enemy drop something (or just make room drop something)
    //TODO: Support for force-spawning waves? (maybe)

    private void Awake()
    {
        for (int i = 1; i < EnemyWaves.Count; i++)
        {
            foreach (GameObject enemy in EnemyWaves[i].Enemies)
            {
                enemy.SetActive(false);
            }
        }

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
            catch (Exception e)
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
