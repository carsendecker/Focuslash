using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

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
    public EmitterContainer emitterObjs;
    
    [Tooltip("List of different combinations of Bullet Attacks.")]
    public List<AttackPattern> AttackPatterns = new List<AttackPattern>();

    private Dictionary<Direction, GameObject> Emitters = new Dictionary<Direction, GameObject>();
    private TaskManager taskManager = new TaskManager();
    private Tree<BossScript> bTree;

    void Awake()
    {
        InitializeBehavior();
    }
    
    void Start()
    {
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
        }
        
        Aggro(false);
        
    }

    void Update()
    {
        taskManager.Update();
    }

    public override void Aggro(bool aggrod)
    {
        if (this.enabled && aggrod)
        {
            var testAttack = new Attack(this, AttackPatterns[0].bulletAttacks);
            taskManager.Do(testAttack);
        }
        
        base.Aggro(aggrod);
    }

    private void InitializeBehavior()
    {
        
    }
    
    #region Nodes

    

    #endregion

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
            Debug.Log("Success!");
        }

        protected override void OnFail()
        {
            
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
