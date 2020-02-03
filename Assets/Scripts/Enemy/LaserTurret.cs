using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class LaserTurret : Creature
{
    public GameObject Laser;
    public ParticleSystem TelegraphParticles, LaserParticles;
    public float AttackFrequency; //How often it attacks
    public float AttackFrequencyVariance; //How random the frequency is
    public float AttackLeadDistance; //How far in front it will aim in front of the player
    public float ChargeTime; //How long it takes to charge
    public float FireTime; //How long it fires for
    public int Damage;
    public Slider HealthBar;
    public GameObject DeathParticles; //For when it dies
    public AudioClip FireSound;

    private PlayerController player;
    private Rigidbody2D playerRb;
    private Collider2D laserCol;
    private float attackTimer;

//    private float maxColY = 0.62f;
//    private bool growCol;
    
    // Start is called before the first frame update
    new void Start() {
        base.Start();
        player = Services.Player;
        playerRb = player.GetComponent<Rigidbody2D>();
        laserCol = Laser.GetComponent<Collider2D>();
        laserCol.enabled = false;
        NewAttackTimer();
        HealthBar.maxValue = MaxHealth;
        HealthBar.value = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                StartCoroutine(Attack());
            }
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
        Vector3 targetPos = player.transform.position + (Vector3)(playerRb.velocity.normalized * AttackLeadDistance);
        var dir = targetPos - Laser.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Laser.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        
        TelegraphParticles.Play();
        yield return new WaitForSeconds(ChargeTime);
        
        TelegraphParticles.Stop();
        TelegraphParticles.Clear();
        
//        yield return new WaitForSeconds(0.5f);
        
        LaserParticles.Play();
        Services.Audio.PlaySound(FireSound, SourceType.EnemySound);
        
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
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
