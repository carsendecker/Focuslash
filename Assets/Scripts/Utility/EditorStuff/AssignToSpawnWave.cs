using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssignToSpawnWave : MonoBehaviour
{
    public int NewWaveNumber;

    [HideInInspector] public EnemySpawner spawner;
    
    /// <summary>
    /// Draws gizmos in scene view to show the wave the enemy is assigned to.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
	    spawner.DrawWEnemyWaveNumbers();

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 20;
        labelStyle.normal.textColor = new Color(0.69f, 1f, 0.46f);
        labelStyle.fontStyle = FontStyle.Bold;
        
        Handles.Label(transform.position, "  *", labelStyle);
        
	}

}
