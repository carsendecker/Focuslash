using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DigManager : MonoBehaviour
{
    public static DigManager DM;
    
    [Header("Generation Sizes")]
    public int RoomCount; //Not including start and end rooms
    public int RoomVariance;
    public int DigPathSize;
    public int DigPathVariance;
    
    [Header("Prefabs and Such")]
    public GameObject StartingRoomPrefab;
    public GameObject EndRoomPrefab;
    public GameObject DiggerPrefab;
    public List<GameObject> RoomList = new List<GameObject>();
    
    [HideInInspector] public Vector2 WallSize;

    private int currentRoomCount;

    private void Awake()
    {
        if(DM == null)
            DM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlaceRoom(StartingRoomPrefab, Vector3.zero);
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        TMP_Text loadText = Services.UI.LoadingScreen.GetComponentsInChildren<TMP_Text>()[0];
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            loadText.text += ".";
        }
        loadText.text = "Press any key to continue";
        yield return new WaitUntil(() => Input.anyKeyDown);
        Services.UI.LoadingScreen.SetActive(false);
        FindObjectOfType<PlayerController>().enabled = true;
    }

    public void CreateDigger(Vector3 position)
    {
        GameObject newDigger = Instantiate(DiggerPrefab, position, Quaternion.identity);
        newDigger.GetComponent<Digger>().TilesToPlace = DigPathSize + Random.Range(-DigPathVariance, DigPathVariance);
    }

    
    private void PlaceRoom(GameObject roomToPlace, Vector3 position)
    {
        Room roomCheck = roomToPlace.GetComponent<Room>();
        if(roomCheck == null)
            Debug.Log("Thats not a room dummy!");

        GameObject newRoom = Instantiate(roomToPlace, position, Quaternion.identity);
        //Center the room on the position
        newRoom.transform.position = new Vector3(
            newRoom.transform.position.x - Mathf.Round(newRoom.GetComponent<Room>().RoomWidth / 2), 
            newRoom.transform.position.y,
            newRoom.transform.position.z);
    }

    //Places a random room if there are still rooms left, or the end room if there aren't
    public void NextRoom(Vector3 position)
    {
        if (currentRoomCount == RoomCount)
        {
            PlaceRoom(EndRoomPrefab, position);
        }
        else
        {
            GameObject randRoom = RoomList[Random.Range(0, RoomList.Count)];
            PlaceRoom(randRoom, position);
            currentRoomCount++;
        }
    }
}
