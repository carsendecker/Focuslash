using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    /*When making rooms:
        0 = air
        1 = wall
        2 = digger
        3+ = tile from TileList
     */
    public TextAsset RoomFile;
    public GameObject WallPrefab;
    //An index number of this list is equal to the number in the RoomGrid - 3 (so TileList[0] = 3 in array)
    public List<GameObject> TileList = new List<GameObject>();
    [HideInInspector] public int RoomWidth, RoomHeight;
    
    protected int[,] RoomGrid;
    protected Vector2 wallSize;
    
    protected void Awake()
    {
        ReadRoom();
        
        RoomWidth = RoomGrid.GetLength(0);
        RoomHeight = RoomGrid.GetLength(1);
        wallSize = WallPrefab.GetComponent<SpriteRenderer>().bounds.size;
    }

    protected void Start()
    {
        GenerateRoom();
    }


    protected virtual void GenerateRoom()
    {
        for (int x = 0; x < RoomWidth; x++)
        {
            for (int y = 0; y < RoomHeight; y++)
            {
                if (RoomGrid[x, y] == 0)
                {
                    Vector3 checkPos = transform.TransformPoint(new Vector3(x * wallSize.x, y * wallSize.y));
                    Collider2D[] check = Physics2D.OverlapPointAll(checkPos);
                    if (check != null)
                    {
                        foreach (var col in check)
                        {
                            if (col.CompareTag("Wall"))
                            {
                                Destroy(col.gameObject);
                            }
                        }
                    }
                }
                else if (RoomGrid[x,y] == 1)
                {
                    Vector3 spawnPos = new Vector3(x * wallSize.x, y * wallSize.y);
                    GameObject newWall = Instantiate(WallPrefab, transform.TransformPoint(spawnPos),Quaternion.identity);
                    newWall.transform.parent = transform;
                }
                else if (RoomGrid[x, y] == 2)
                {
                    Vector3 spawnPos = new Vector3(x * wallSize.x, y * wallSize.y);
                    DigManager.DM.CreateDigger(transform.TransformPoint(spawnPos));
                }
                else if(RoomGrid[x,y] - 3 < TileList.Count)
                {
                    GameObject obj = TileList[RoomGrid[x,y] - 3];
                    Vector3 spawnPos = new Vector3(x * wallSize.x, y * wallSize.y);
                    GameObject newTile = Instantiate(obj, transform.TransformPoint(spawnPos), Quaternion.identity);

                    if (!newTile.CompareTag("Player"))
                    {
                        newTile.transform.parent = transform;
                    }
                }
            }
        }
    }
    
    //Text file parsing code taken from Stone Soup :O
    private void ReadRoom()
    {
        string initialGridString = RoomFile.text;
        
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        
        int height = rows.Length;
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++) {
            string row = rows[height-r-1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++) {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        RoomGrid = indexGrid;
    }
}
