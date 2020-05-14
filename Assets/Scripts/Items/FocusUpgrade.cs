using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusUpgrade : Creature
{
    public int FocusIncreaseNumber;
    public GameObject LevelUpParticles;
    public GameObject DeathParticles;
    public float DeathParticleMoveSpeed = 3;
    public float floatingMaxOffset;
    public AudioClip PickupSound;

    //Stuff for the cool obtainment "cutscene"
    private ParticleSystem[] particles;
    private bool dead;
    private ParticleSystem obtainParticles;
    private ParticleSystem.Particle[] particleList;
    private int aliveParticles;
    private bool particlesToPlayer;
    private Vector2 startingPos;

    private void OnEnable()
    {
        particles = transform.GetComponentsInChildren<ParticleSystem>();
        startingPos = transform.position;
    }

    private void FixedUpdate()
    {
        //Track the particles towards the player after picking up the item
        if (particlesToPlayer)
        {
            aliveParticles = obtainParticles.GetParticles(particleList);
            
            for (int i = 0; i < aliveParticles; i++)
            {
                particleList[i].velocity = (Services.Player.transform.position - particleList[i].position).normalized * DeathParticleMoveSpeed;

                if (Vector2.Distance(particleList[i].position, Services.Player.transform.position) < 0.1f)
                {
                    particleList[i].startSize = 0f;
                }
            }

            obtainParticles.SetParticles(particleList, aliveParticles);
        }
    }

    private void Update()
    {
        if(!dead)
            ItemFloat();
    }

    public void AddMoreFocus(int focusToAdd)
    {
        Services.Player.AttackCount += focusToAdd;
        Services.Player.CurrentFocus = Services.Player.AttackCount;
        Instantiate(LevelUpParticles, Services.Player.transform);

    }

    protected override void Die()
    {
        if (!dead)
        {
            Services.Audio.PlaySound(PickupSound, SourceType.CreatureSound);
            Destroy(GetComponent<Collider2D>());
            StartCoroutine(ObtainmentCeremony());
        }
    }

    IEnumerator ObtainmentCeremony()
    {
        //Slow down time and stop all the other particles on the object
        dead = true;
        Time.timeScale = 0.1f;
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
        
        obtainParticles = Instantiate(DeathParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particleList = new ParticleSystem.Particle[obtainParticles.main.maxParticles];
        
        yield return new WaitForSecondsRealtime(2f);
        
        Time.timeScale = 1;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSecondsRealtime(1.25f);

        // aliveParticles = obtainParticles.GetParticles(particleList);
        ParticleSystem.LimitVelocityOverLifetimeModule lv = obtainParticles.limitVelocityOverLifetime;
        lv.enabled = false;

        particlesToPlayer = true;

        yield return new WaitForSecondsRealtime(0.4f);
        
        AddMoreFocus(FocusIncreaseNumber);
        Services.UI.UpdatePlayerFocus();
        
        yield return new WaitForSecondsRealtime(2f);

        Destroy(obtainParticles.gameObject);
        Destroy(gameObject);
    }

    private bool floatingUp = true;
    void ItemFloat()
    {
        Vector2 tempPos = transform.position;
        
        if (floatingUp && transform.position.y < startingPos.y + (floatingMaxOffset - 0.1f))
        {
            tempPos.y = Mathf.Lerp(tempPos.y, startingPos.y + floatingMaxOffset, 0.01f);
        }
        else if(floatingUp)
        {
            floatingUp = false;
        }

        if (!floatingUp && transform.position.y > startingPos.y)
        {
            tempPos.y = Mathf.Lerp(tempPos.y, startingPos.y - 0.1f, 0.01f);
        }
        else if(!floatingUp)
        {
            floatingUp = true;
        }

        transform.position = tempPos;

    }
    
    
}
