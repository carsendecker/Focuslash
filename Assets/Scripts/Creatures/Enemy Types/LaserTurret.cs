using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class LaserTurret : Enemy
{
    public GameObject Laser;
    public ParticleSystem TelegraphParticles, LaserParticles;
    public float AttackFrequency; //How often it attacks
    public float AttackFrequencyVariance; //How random the frequency is
    public float AttackLeadDistance; //How far in front it will aim in front of the player
    public float ChargeTime; //How long it takes to charge
    public float FireTime; //How long it fires for
    public Slider HealthBar;
    public GameObject DeathParticles; //For when it dies
    public AudioClip FireSound;

    private Rigidbody2D playerRb;
    private Collider2D laserCol;
    private float attackTimer;
    private bool charging;
    private Vector3 targetPos;

//    private float maxColY = 0.62f;
//    private bool growCol;
    
    protected override void Start() {
        base.Start();
        playerRb = Services.Player.GetComponent<Rigidbody2D>();
        laserCol = Laser.GetComponent<Collider2D>();
        laserCol.enabled = false;
        
        HealthBar.maxValue = MaxHealth;
        HealthBar.value = MaxHealth;
        
        NewAttackTimer();

    }

    void Update()
    {
        //Begins an attack after a cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                StartCoroutine(Attack());
            }
        }
        else if (charging)
        {
            targetPos = Vector3.Lerp(targetPos, Services.Player.transform.position, 0.3f);
            var dir = targetPos - Laser.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Laser.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        HealthBar.value = Mathf.Lerp(HealthBar.value, health, 0.1f);
    }

    private void NewAttackTimer()
    {
        float randVariance = Random.Range(-AttackFrequencyVariance, AttackFrequencyVariance);
        attackTimer = AttackFrequency + randVariance;
    }

    private IEnumerator Attack()
    {
        charging = true;
        targetPos = Services.Player.transform.position;

        TelegraphParticles.Play();
        yield return new WaitForSeconds(ChargeTime - 0.3f);
        charging = false;
        yield return new WaitForSeconds(0.3f);

        TelegraphParticles.Stop();
        TelegraphParticles.Clear();

        
        LaserParticles.Play();
        Services.Audio.PlaySound(FireSound, SourceType.CreatureSound);
        
        yield return new WaitForSeconds(FireTime);

        LaserParticles.Stop();
        NewAttackTimer();
        
    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            player.TakeDamage(Damage);
//        }
//    }
    
//    private void OnParticleCollision(GameObject other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            player.TakeDamage(Damage);
//        }
//    }

    protected override void Die()
    {
        base.Die();
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
