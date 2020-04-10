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

    //Stuff for the cool obtainment "cutscene"
    private ParticleSystem[] particles;
    private bool dead;
    private ParticleSystem obtainParticles;
    private ParticleSystem.Particle[] particleList;
    private int aliveParticles;
    private bool particlesToPlayer;

    private void OnEnable()
    {
        particles = transform.GetComponentsInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
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
    
    
}
