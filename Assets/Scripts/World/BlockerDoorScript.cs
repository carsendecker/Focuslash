using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDoorScript : Creature
{
    public bool startClosed;
    public AudioClip DoorSound;
    private SpriteRenderer thisSpriteRenderer;
    
    
    void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.color = Color.gray;

        gameObject.SetActive(startClosed);
    }

    public void closeDoorWay()
   {
       //Set the game object to active here.
        gameObject.SetActive(true);
        tag = "Wall";
        Services.Audio.PlaySound(DoorSound, SourceType.AmbientSound);
   }

   public void makeDoorSlashable()
   {
       thisSpriteRenderer.color = new Color(0.24f, 0.87f, 0.87f);
       tag = "Untagged";
       Services.Audio.PlaySound(DoorSound, SourceType.AmbientSound);

   }
}
