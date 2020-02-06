using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigKnight : Creature
{
    [Header("Stats")]
    public float KnockbackForce;
    public float ChargeTime;

    [Header("Components")]
    public Slider HealthBar;
    public Slider TelegraphSlider;
    public GameObject DeathParticles;
    public GameObject AttackObject;

    private Vector3 direction;
    private bool isMoving;
    private float chargeAmount;
    
    // Start is called before the first frame update
    void Start()
    {
        TelegraphSlider.maxValue = ChargeTime;
        TelegraphSlider.value = 0;
        TelegraphSlider.gameObject.SetActive(false);

        HealthBar.maxValue = MaxHealth;
        HealthBar.value = MaxHealth;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            direction = Vector3.Lerp(direction, Services.Player.transform.position - transform.position, 0.1f);
            rb.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            rb.velocity = transform.right * MoveSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.value = Mathf.Lerp(HealthBar.value, health, 0.2f);
    }

    void Attack()
    {
        isMoving = false;
        TelegraphSlider.gameObject.SetActive(true);

        while (chargeAmount < ChargeTime)
        {
            chargeAmount += Time.deltaTime;
            TelegraphSlider.value = chargeAmount;
        }

        chargeAmount = 0;
        TelegraphSlider.gameObject.SetActive(false);
        AttackObject.SetActive(true);
        
        isMoving = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //When player enters their range, stop and attack;
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }

    protected override void Die()
    {
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
