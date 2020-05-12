using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDoorScript : Creature
{
    public bool startClosed;
    public bool startSlashable;
    public AudioClip DoorSound;
    public Sprite OpenSprite, ClosedSprite;
    
    private SpriteRenderer thisSpriteRenderer;
    private Collider2D col;
    
    
    void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.color = Color.gray;
        col = GetComponent<Collider2D>();

        col.enabled = startClosed;
        if (startClosed)
            thisSpriteRenderer.sprite = ClosedSprite;
        else
            thisSpriteRenderer.sprite = OpenSprite;
        
        if(startSlashable)
            makeDoorSlashable();
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
       thisSpriteRenderer.color = new Color(0.25f, 1f, 1f);
       tag = "Untagged";
   }

   protected override void Die()
   {
       col.enabled = false;
       thisSpriteRenderer.sprite = OpenSprite;
   }
}
