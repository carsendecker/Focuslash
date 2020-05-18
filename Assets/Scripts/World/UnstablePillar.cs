using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstablePillar : MonoBehaviour
{
    public Sprite CrackedSprite;
    public ParticleSystem ExplodeParticles;
    public AudioClip ExplodeSound;
    
    private bool hit;
    private SpriteRenderer sr;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Lethal") && !hit)
        {
            hit = true;
            StartCoroutine(Break());
        }
    }

    private IEnumerator Break()
    {
        yield return new WaitForSeconds(1.75f);

        sr.sprite = CrackedSprite;
        
        yield return new WaitForSeconds(2f);

        Instantiate(ExplodeParticles, transform.position, Quaternion.identity);
        Services.Audio.PlaySound(ExplodeSound, SourceType.AmbientSound);
        
        Destroy(gameObject);
    }
}
