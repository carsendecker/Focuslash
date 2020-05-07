using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    public EmitterContainer emitterObjs;
    
    [Tooltip("List of different combinations of Bullet Attacks.")]
    public List<AttackPattern> AttackPatterns = new List<AttackPattern>();

    private Dictionary<Direction, GameObject> Emitters = new Dictionary<Direction, GameObject>();
    private TaskManager taskManager = new TaskManager();
    private FiniteStateMachine<BossScript> state;
    private List<Attack> Attacks = new List<Attack>();
    private SpriteRenderer sr;

    private bool invulnerable;
    
    void Awake()
    {
        state = new FiniteStateMachine<BossScript>(this);
        sr = GetComponentInChildren<SpriteRenderer>();
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
        
    }

    protected override void Update()
    {
        base.Update();
        state.Update();
    }


    public override void Aggro(bool aggrod)
    {
        if (this.enabled && aggrod)
        {
            Debug.Log("Going to Idle...");
            state.TransitionTo<Idle>();
        }
        
        base.Aggro(aggrod);
    }

    public override bool TakeDamage(int damage)
    {
        if (invulnerable)
        {
            return false;
        }
        
        return base.TakeDamage(damage);
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
    
    private class Idle : FiniteStateMachine<BossScript>.State
    {
        private float cooldownTimer;
        private byte timesCharged;

        public override void OnEnter()
        {
            cooldownTimer = Context.AttackCooldown;
        }

        public override void Update()
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                //When at or below 2/3 HP and 1/3 HP, charge at the player
                if ((Context.health <= Context.MaxHealth * (2/3) && timesCharged == 0) ||
                    (Context.health <= Context.MaxHealth * (1/3) && timesCharged == 1))
                {
                    timesCharged++;
                    TransitionTo<Charging>();
                }
                else
                {
                    TransitionTo<Shooting>();
                }
            }
        }
    }

    private class Shooting : FiniteStateMachine<BossScript>.State
    {
        private Queue<int> previousAttacks = new Queue<int>();
        private int nextAttack;
            
        public override void OnEnter()
        {
            Context.sr.color = Color.gray;
            Context.gameObject.tag = "EnemyWall";
            Context.invulnerable = true;
            ChooseNextAttack();
        }

        public override void OnExit()
        {
            Context.sr.color = Color.white;
            Context.gameObject.tag = "Enemy";
            Context.invulnerable = false;
        }

        public override void Update()
        {
            Context.taskManager.Update();
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
    
    private class Charging : FiniteStateMachine<BossScript>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Is going to charge!");
            TransitionTo<Idle>();
        }

        public override void Update()
        {
            
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
            Debug.Log("Initialized!");
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
