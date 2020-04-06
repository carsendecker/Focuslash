using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerTool : Editor
{
	private EnemySpawner spawner;
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		spawner = (EnemySpawner) target;

		int index = 0;
        foreach (Wave wave in spawner.EnemyWaves)
        {
            foreach (GameObject enemy in wave.Enemies)
            {
	            AssignToSpawnWave sw = enemy.GetComponent<AssignToSpawnWave>();
	            if (sw == null)
	            {
		            sw = enemy.AddComponent<AssignToSpawnWave>();
	            }
	            
	            sw.spawner = spawner;
            }

            index++;
        }
	}
}
