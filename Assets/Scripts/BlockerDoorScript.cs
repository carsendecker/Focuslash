using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDoorScript : Creature
{
    public bool startClosed;
    private SpriteRenderer thisSpriteRenderer;
    
    
    void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.color = Color.gray;
        
        //Add the method to the event currently on the DoorEvents script
        //The video I'm using called it subscribing
        // DoorEvents.current.onDoorwayTriggerEnter += closeDoorWay;
        // DoorEvents.current.onEnemiesDefeated += makeDoorSlashable;
        
        gameObject.SetActive(startClosed);
    }

    public void closeDoorWay()
   {
       //Set the game object to active here.
        gameObject.SetActive(true);
        tag = "Wall";
   }

   public void makeDoorSlashable()
   {
       thisSpriteRenderer.color = new Color(0.24f, 0.87f, 0.87f);
       tag = "Untagged";
   }
}
