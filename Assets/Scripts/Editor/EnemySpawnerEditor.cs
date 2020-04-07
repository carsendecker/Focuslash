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

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		spawner = (EnemySpawner) target;
		List<GameObject> enemyList = new List<GameObject>();
		
		string[] prefabPaths = Directory.GetFiles(Application.dataPath + "/Prefabs/Creatures/Enemies", "*.prefab");
		foreach (string path in prefabPaths)
		{
			enemyList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path.Substring(path.IndexOf("Assets"))));
		}

		string[] nameList = new string[enemyList.Count];
		for (int i = 0; i < enemyList.Count; i++)
		{
			nameList[i] = enemyList[i].name;
		}

		//Dropdown and button for spawning enemies into the room
		GUILayout.BeginHorizontal();

		enemyIndex = EditorGUILayout.Popup("Spawn Enemy: ", enemyIndex, nameList);
		
		if (GUILayout.Button("Spawn Enemy in Room"))
		{
			foreach (GameObject enemy in enemyList)
			{
				if (enemy.name == nameList[enemyIndex])
				{
					GameObject o = Instantiate(enemy, spawner.transform);
					o.AddComponent<AssignToSpawnWave>();
					spawner.AssignToWave(o, 0);
				}
			}
		}
		
		GUILayout.EndHorizontal();

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
