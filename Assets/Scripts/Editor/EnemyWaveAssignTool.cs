using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssignToSpawnWave))]
[CanEditMultipleObjects]
public class EnemyWaveAssignTool : Editor
{
	private AssignToSpawnWave[] enemies;
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		enemies = Array.ConvertAll(targets, item => (AssignToSpawnWave) item);

		if (GUILayout.Button("Assign to Wave " + enemies[0].NewWaveNumber))
		{
			foreach (AssignToSpawnWave enemy in enemies)
			{
				if (enemy.NewWaveNumber == enemy.spawner.GetWaveNumber(enemy.gameObject)) return;
			
				enemy.spawner.AssignToWave(enemy.gameObject, enemy.NewWaveNumber);
			}
			
			SceneView.RepaintAll();
		}

	}
}
