using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BossScript : Enemy
{
    [Serializable]
    public struct AttackPattern
    {
        public BulletAttack[] bulletAttacks;
    }

    [Serializable]
    public struct EmitterContainer
    {
        public GameObject up, down, left, right;
    }
    
    public Transform EmitterParent;
    public float AttackCooldown;
    public Rigidbody2D ShieldParent, LaserParent;
    public EmitterContainer emitterObjs;
    public AudioClip deathSound, whooshSound, chargeSound, fireSound;
    public ParticleSystem ChargeParticles, LaserParticles, TelegraphParticles;

    public AttackPattern OpeningAttack;
    [Tooltip("List of different combinations of Bullet Attacks.")]
    public List<AttackPattern> AttackPatterns = new List<AttackPattern>();

    private Dictionary<Direction, GameObject> Emitters = new Dictionary<Direction, GameObject>();
    private TaskManager taskManager = new TaskManager();
    private FiniteStateMachine<BossScript> state;
    private List<Attack> Attacks = new List<Attack>();
    private SpriteRenderer sr;

    private bool invulnerable;
    private Vector2 direction;
    private bool shieldUp;
    
    void Awake()
    {
        state = new FiniteStateMachine<BossScript>(this);
        sr = GetComponentInChildren<SpriteRenderer>();
        ShieldParent.gameObject.SetActive(false);
    }
    
    protected override void Start()
    {
        base.Start();
        
        Emitters.Add(Direction.Up, emitterObjs.up);
        Emitters.Add(Direction.Down, emitterObjs.down);
        Emitters.Add(Direction.Left, emitterObjs.left);
        Emitters.Add(Direction.Right, emitterObjs.right);

        //Pools all prefabs
        foreach (var attack in AttackPatterns)
        {
            foreach (var pattern in attack.bulletAttacks)
            {
                Services.ObjectPools.Create(pattern.BulletPrefab, 50);
            }
            
            //Adds all attacks into a list of Attack tasks to choose from later
            Attacks.Add(new Attack(this, attack.bulletAttacks));
        }
        
        state.TransitionTo<Inactive>();

        Aggro(false);
        
        Services.Events.Register<PlayerLeftAttackPhase>(PutUpShield);
        
    }

    protected override void Update()
    {
        base.Update();
        state.Update();

        if (!Services.Player.IsPhase(PlayerController.Phase.Attacking))
        {
            Vector3 targetPos = Services.Player.transform.position;

            direction = Vector3.Lerp(direction, targetPos - transform.position, 0.04f);
            ShieldParent.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }

    private void GoInvincible(bool enabled)
    {
        if (enabled)
        {
            sr.color = Color.gray;
            gameObject.tag = "EnemyWall";
            invulnerable = true;
        }
        else
        {
            sr.color = Color.white;
            gameObject.tag = "Enemy";
            invulnerable = false;
        }
    }


    public override void Aggro(bool aggrod)
    {
        if (this.enabled && aggrod)
        {
            state.TransitionTo<Opener>();
        }
        
        base.Aggro(aggrod);
    }

    public override bool TakeDamage(int damage)
    {
        if (invulnerable)
        {
            return false;
        }

        if (shieldUp)
        {
            ShieldParent.gameObject.SetActive(false);
        }
        
        return base.TakeDamage(damage);
    }

    private void PutUpShield(AGPEvent e)
    {
        if (!shieldUp) return;
        
        ShieldParent.gameObject.SetActive(true);
    }

    protected override void Die()
    {
        StartCoroutine(DeathCutscene());
    }

    /// <summary>
    /// Slows down time & fades to white when the boss dies
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathCutscene()
    {
        Time.timeScale = 0f;
        Services.Audio.PlaySound(deathSound, SourceType.CreatureSound);
        Services.Audio.StopMusic();

        Services.UI.CameraOverlay.color = new Color(1f, 1f, 1f, 0.75f);
        Services.UI.CameraOverlay.enabled = true;
        
        yield return new WaitForSecondsRealtime(1.75f);

        Time.timeScale = 0.01f;
        
        Services.Audio.PlaySound(whooshSound, SourceType.AmbientSound);

        float t = 0;
        while (t < 1f)
        {
            Services.UI.FullOverlay.enabled = true;
            Services.UI.FullOverlay.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.fixedDeltaTime * 0.15f;
            yield return 0;
        }
        
        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(5);
    }


    //=================================================
    // STATE MACHINE
    //=================================================
    
    #region States

    private class Inactive : FiniteStateMachine<BossScript>.State
    {
        //Do nothing lol
        
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
        }
    }
    
    private class Opener : FiniteStateMachine<BossScript>.State
    {
        public override void OnEnter()
        {
            Attack opener = new Attack(Context, Context.OpeningAttack.bulletAttacks);
            Context.taskManager.Do(opener);
        }

        public override void Update()
        {
            Context.taskManager.Update();
        }
    }
    
    private class Idle : FiniteStateMachine<BossScript>.State
    {
        private float cooldownTimer;
        private byte timesCharged = 0;

        public override void OnEnter()
        {
            cooldownTimer = Context.AttackCooldown;
        }

        public override void Update()
        {
            cooldownTimer -= Time.deltaTime;
            
            if (cooldownTimer <= 0)
            {
                //When at or below 2/3 HP and 1/3 HP, shoot laser at the player
                if ((Context.health <= Context.MaxHealth * (2f/3f) && timesCharged == 0) ||
                    (Context.health <= Context.MaxHealth * (1f/3f) && timesCharged == 1))
                {
                    timesCharged++;
                    TransitionTo<SuperLaser>();
                }
                else if (!Context.shieldUp && Context.health <= Context.MaxHealth / 2f)
                {
                    Context.shieldUp = true;
                    Context.ShieldParent.gameObject.SetActive(true);
                }
                else
                {
                    TransitionTo<Shooting>();
                }
            }
        }
    }

    /// <summary>
    /// The state the boss enters when its... shooting
    /// </summary>
    private class Shooting : FiniteStateMachine<BossScript>.State
    {
        private Queue<int> previousAttacks = new Queue<int>();
        private int nextAttack;
        private bool pausing;
            
        public override void OnEnter()
        {
            Context.GoInvincible(true);

            Context.StartCoroutine(PauseUntilAttack());
        }

        public override void OnExit()
        {
            Context.GoInvincible(false);
        }

        public override void Update()
        {
            if (pausing) return;
            
            Context.taskManager.Update();
        }

        private IEnumerator PauseUntilAttack()
        {
            pausing = true;
            
            yield return new WaitForSeconds(1f);
            
            ChooseNextAttack();
            pausing = false;
        }

        private void ChooseNextAttack()
        {
            do
            {
                nextAttack = Random.Range(0, Context.AttackPatterns.Count);
            } while (previousAttacks.Contains(nextAttack));

            if(previousAttacks.Count == 2)
                previousAttacks.Dequeue();
            
            previousAttacks.Enqueue(nextAttack);

            Context.taskManager.Do(Context.Attacks[nextAttack]);
        }
        
    }
    
    /// <summary>
    /// Enters this state at 2/3 and 1/3 HP, charges and fires a laser that breaks a pillar
    /// </summary>
    private class SuperLaser : FiniteStateMachine<BossScript>.State
    {
        private Vector3 targetPos;
        private bool wasShieldUp;
        
        public override void OnEnter()
        {
            Context.GoInvincible(true);

            wasShieldUp = Context.shieldUp;
            
            if (wasShieldUp)
            {
                Context.ShieldParent.gameObject.SetActive(false);
                Context.shieldUp = false;
            }

            Context.StartCoroutine(FireLaser());
        }

        public override void OnExit()
        {
            Context.GoInvincible(false);
            
            if (wasShieldUp)
            {
                Context.ShieldParent.gameObject.SetActive(true);
                Context.shieldUp = true;
            }
        }

        private IEnumerator FireLaser()
        {
            Context.ChargeParticles.Play();
            Context.TelegraphParticles.Play();
            Services.Audio.PlaySound(Context.chargeSound, SourceType.CreatureSound);
            
            yield return new WaitForSeconds(Context.ChargeParticles.main.duration);
            
            Context.ChargeParticles.Stop();
            Context.TelegraphParticles.Stop();
            
            yield return new WaitForSeconds(0.4f);
            
            Context.LaserParticles.Play();
            Services.Audio.PlaySound(Context.fireSound, SourceType.CreatureSound);
            Services.Utility.ShakeCamera(Context.LaserParticles.main.duration, 0.5f);

            yield return new WaitForSeconds(Context.LaserParticles.main.duration);
            
            Context.LaserParticles.Stop();
            
            yield return new WaitForSeconds(1f);
            
            TransitionTo<Idle>();
        }
        

        public override void Update()
        {
            targetPos = Vector3.Lerp(targetPos, Services.Player.transform.position, 0.3f);
            var dir = targetPos - Context.LaserParent.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Context.LaserParent.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
        
    }

    #endregion

    
    
    //=================================================
    // TASK MANAGER TASKS
    //=================================================

    #region Tasks
    
    private class Attack : Task
    {
        private BossScript boss;
        private BulletAttack[] attacks;
        private List<Coroutine> AttackCoroutines = new List<Coroutine>();

        private int currentAttack;
        private float duration;

        public Attack(BossScript boss, BulletAttack[] attacks)
        {
            this.boss = boss;
            this.attacks = attacks;
        }
        
       
        protected override void Initialize()
        {
            currentAttack = 0;
            duration = 0;
            
            //Resets emitter positions
            boss.EmitterParent.rotation = Quaternion.identity;
        }
        
        internal override void Update()
        {
            //If we've run out of bullet patterns to fire, the attack is done
            if (currentAttack >= attacks.Length)
            {
                SetStatus(TaskStatus.Success);
                return;
            }

            if (FireBulletAttack(attacks[currentAttack]))
            {
                currentAttack++;
                duration = 0;
                coroutinesStarted = false;
            }

        }

        protected override void OnSuccess()
        {
            boss.state.TransitionTo<Idle>();
        }
        

        /// <summary>
        /// Takes a BulletAttack object and fires the attack according to them specific specifications
        /// </summary>
        private bool coroutinesStarted;
        private bool FireBulletAttack(BulletAttack attack)
        {
            //Fire until the duration is up
            if (duration >= attack.AttackDuration)
            {
                foreach (Coroutine coroutine in AttackCoroutines)
                {
                    boss.StopCoroutine(coroutine);
                }
                return true;
            }
            
            //Shoot each pattern in the attack at their fire rate
            if (!coroutinesStarted)
            {
                Services.Audio.PlaySound(attack.FireSound, SourceType.CreatureSound);
                
                foreach (var pattern in attack.Patterns)
                {
                    AttackCoroutines.Add(boss.StartCoroutine(Shoot(attack.BulletPrefab, pattern)));
                }
                coroutinesStarted = true;
            }

            boss.EmitterParent.Rotate(new Vector3(0, 0, attack.RotationSpeed));

            duration += Time.deltaTime;
            return false;
        }

        /// <summary>
        /// Actually enables and shoots the bullets based on a BulletPattern
        /// </summary>
        private IEnumerator Shoot(GameObject prefab, BulletAttack.BulletPattern pattern)
        {
            while (true)
            {
                Vector3 spawnPos = boss.Emitters[pattern.Direction].transform.position;

                if (pattern.Direction == Direction.Up || pattern.Direction == Direction.Down)
                    spawnPos.x += pattern.Offset;
                else
                    spawnPos.y += pattern.Offset;

                //Spawn and rotate in correct direction
                GameObject obj = Services.ObjectPools.Spawn(prefab, spawnPos, Quaternion.identity);
                obj.SetActive(false); //ew sorry
                obj.GetComponent<BulletScript>().MoveSpeed = pattern.BulletSpeed;
                obj.transform.rotation = boss.Emitters[pattern.Direction].transform.rotation;
                obj.transform.Rotate(new Vector3(0, 0, pattern.Angle));
                obj.SetActive(true);
                
                yield return new WaitForSeconds(1/pattern.FireRate);
            }
        }
        
        
    }
    
    #endregion

}
