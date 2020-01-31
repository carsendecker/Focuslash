using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaRoom : Room
{
    public AudioClip doorCloseSound;
    
    private EnemySpawner spawner;
    private Collider2D col;
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        spawner = GetComponent<EnemySpawner>();
        spawner.enabled = false;
        col = gameObject.AddComponent<BoxCollider2D>(); //collider to detect player entry
        col.isTrigger = true;
        col.offset = new Vector2(RoomWidth / 2, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SystemsManager.UI.ScoreText.gameObject.SetActive(true);
            Instantiate(WallPrefab, transform.localPosition + new Vector3(RoomWidth / 2, 0f, 0f), Quaternion.identity);
            Destroy(col);
            SystemsManager.Audio.PlaySound(doorCloseSound, SourceType.EnemySound);
            
            spawner.MAP_WIDTH = RoomWidth;
            spawner.MAP_HEIGHT = RoomHeight;
            spawner.enabled = true;
        }
    }
}
