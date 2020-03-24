using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDoorScript : MonoBehaviour
{
    private SpriteRenderer thisSpriteRenderer;
    
    
    void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.color = Color.gray;
        
        //Add the method to the event currently on the DoorEvents script
        //The video I'm using called it subscribing
        DoorEvents.current.onDoorwayTriggerEnter += closeDoorWay;
        DoorEvents.current.onEnemiesDefeated += makeDoorSlashable;
        
        this.gameObject.SetActive(false);
    }

    
    void Update()
    {
        
    }
    
    public void closeDoorWay()
   {
       //Set the game object to active here.
        this.gameObject.SetActive(true);
        this.gameObject.tag = "Wall";
   }

   public void makeDoorSlashable()
   {
       thisSpriteRenderer.color = Color.blue;
       this.gameObject.SetActive(false);

   }
}
