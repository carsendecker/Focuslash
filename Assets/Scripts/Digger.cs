using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger : MonoBehaviour
{
    private enum Facing
    {
        Up = 0,
        Left = 1,
        Right = 2
    }
    
    public GameObject WallPrefab;
    public float ClearingChance;
    public int ClearingWidth, ClearingHeight;
    public float TurnLeftChance, TurnRightChance;
    public int RoomLeadUpTiles;
    
    [HideInInspector] public int TilesToPlace;

    private DigManager dm;
    private Vector2 wallSize;
    private int tilesLeft;
    private Facing orientation;
    private bool placing = true;
    private List<GameObject> walls = new List<GameObject>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        dm = DigManager.DM;
        wallSize = WallPrefab.GetComponent<SpriteRenderer>().bounds.size;
        tilesLeft = TilesToPlace;
        orientation = Facing.Up;
    }

    // Update is called once per frame
    void Update()
    {
        if (!placing) return;
        
        if (Random.value <= ClearingChance) {
            for (int xOffset = 0; xOffset < ClearingWidth; xOffset++) {
                for (int yOffset = 0; yOffset < ClearingHeight; yOffset++) {
                    Vector2 floorPosition = (Vector2)transform.position + (Vector2.right * xOffset * wallSize) + (Vector2.up * yOffset * wallSize);
                    placeWall(floorPosition);
                }
            }
        }
        else {
            placeWall(transform.position);
        }

        
        Vector3 newPos = transform.up * wallSize;
        transform.position += newPos;

        //Go straight for the first couple blocks
        if (tilesLeft < 3 || tilesLeft > TilesToPlace - 3)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        float randomNumber = Random.value;
        if (randomNumber <= TurnLeftChance) {
            //Don't move backwards
            if (orientation != Facing.Left)
            {
                transform.Rotate(0, 0, 90);
                
                //If facing up, is now left
                if (orientation == Facing.Up)
                    orientation = Facing.Left;
                //If facing right, is now up
                else
                    orientation = Facing.Up;
            }
        }
        else if (randomNumber <= TurnLeftChance + TurnRightChance) {
            if (orientation != Facing.Right)
            {
                transform.Rotate(0, 0, -90);
                
                //If facing up, is now right
                if (orientation == Facing.Up)
                    orientation = Facing.Right;
                //If facing left, is now up
                else
                    orientation = Facing.Up;
            }
        }
    }
    
    private void placeWall(Vector2 floorPosition)
    {
        GameObject newWall = PlaceWallCheck(floorPosition);
        if (newWall != null)
        {
            walls.Add(newWall);
            tilesLeft--;
        }
        
        if (tilesLeft <= 0)
        {
            transform.rotation = Quaternion.identity;
            StartCoroutine(ConvertToFloor());
        }
    }

    GameObject PlaceWallCheck(Vector3 spawnPos)
    {
        Collider2D[] check = Physics2D.OverlapPointAll(spawnPos);
        if (check == null || check.Length == 0)
        {
            return Instantiate(WallPrefab, spawnPos, Quaternion.identity);
            
        }

        bool noWalls = true;
        foreach (var col in check)
        {
            if (col.CompareTag("Wall"))
            {
                noWalls = false;
            }
        }

        if (noWalls)
        {
            return Instantiate(WallPrefab, spawnPos, Quaternion.identity);
        }

        return null;
    }

    IEnumerator ConvertToFloor()
    {
        placing = false;
        foreach (var wall in walls)
        {
            if (walls.IndexOf(wall) == 0) continue;
            
            Vector3 spawnPos = wall.transform.position + (Vector3)(wall.transform.right * wallSize);
            PlaceWallCheck(spawnPos);
            
            spawnPos = wall.transform.position + (Vector3)(-wall.transform.right * wallSize);
            PlaceWallCheck(spawnPos);

            spawnPos = wall.transform.position + (Vector3)(wall.transform.up * wallSize);
            PlaceWallCheck(spawnPos);

            spawnPos = wall.transform.position + (Vector3)(-wall.transform.up * wallSize);
            PlaceWallCheck(spawnPos);


            yield return new WaitForEndOfFrame();
        }

        GameObject[] wallsArray = walls.ToArray();
        for(int i = wallsArray.Length - 1; i >= 0; i--)
        {
            Destroy(wallsArray[i]);
        }
        walls.Clear();
        dm.NextRoom(transform.position);
        Destroy(gameObject);
        
    }

//    void FinishUp()
//    {
//        foreach (var wall in walls)
//        {
//            Destroy(wall);
//        }
//        dm.NextRoom(transform.position);
//        Destroy(gameObject);
//    }
}
