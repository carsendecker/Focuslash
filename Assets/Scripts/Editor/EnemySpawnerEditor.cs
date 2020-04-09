using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
	private EnemySpawner spawner;
	private int enemyIndex;
	private int enemyNumToSpawn = 1;
	private int enemyWaveToSpawn = 0;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		spawner = (EnemySpawner) target;
		List<GameObject> enemyList = new List<GameObject>();
		
		//Gets the file paths for all the enemy prefabs and adds the prefabs to a list
		string[] prefabPaths = Directory.GetFiles(Application.dataPath + "/Prefabs/Creatures/Enemies", "*.prefab");
		foreach (string path in prefabPaths)
		{
			enemyList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path.Substring(path.IndexOf("Assets"))));
		}

		//Makes a list of the enemy names
		string[] nameList = new string[enemyList.Count];
		for (int i = 0; i < enemyList.Count; i++)
		{
			nameList[i] = enemyList[i].name;
		}
		
		//==========Inspector section that allows for spawning an enemy into the room======//
		GUILayout.Space(15);
		GUILayout.Label("Spawn Enemy into Room:");
		enemyWaveToSpawn = EditorGUILayout.IntField("Assign to wave: ", enemyWaveToSpawn);
		
		GUILayout.BeginHorizontal();
		enemyIndex = EditorGUILayout.Popup(enemyIndex, nameList);
		enemyNumToSpawn = EditorGUILayout.IntField(enemyNumToSpawn);
		GUILayout.EndHorizontal();
		
		
		//SPAWNING AN ENEMY INTO THE ROOM BUTTON
		if (GUILayout.Button("Spawn Enemy in Room"))
		{
			foreach (GameObject enemy in enemyList)
			{
				if (enemy.name == nameList[enemyIndex])
				{
					for (int i = 0; i < enemyNumToSpawn; i++)
					{
						GameObject o = Instantiate(enemy, spawner.transform);
						o.AddComponent<AssignToSpawnWave>();
						spawner.AssignToWave(o, enemyWaveToSpawn);
						Selection.activeGameObject = o;
					}
				}
			}
		}
		
		EditorGUILayout.HelpBox("Choose an enemy type from the dropdown, and specify how many you want to spawn. " +
		                        "They will then be instantiated as a child of the room and added to wave 0 automatically.", MessageType.Info);
		
		//Gives the AssignToSpawnWave component if any enemies do not have them
		// int index = 0;
  //       foreach (Wave wave in spawner.EnemyWaves)
  //       {
  //           foreach (GameObject enemy in wave.Enemies)
  //           {
	 //            if(enemy == null) continue;
	 //            AssignToSpawnWave sw = enemy.GetComponent<AssignToSpawnWave>();
	 //            if (sw == null)
	 //            {
		//             sw = enemy.AddComponent<AssignToSpawnWave>();
	 //            }
	 //            
	 //            sw.spawner = spawner;
  //           }
  //
  //           index++;
  //       }
	}
}
