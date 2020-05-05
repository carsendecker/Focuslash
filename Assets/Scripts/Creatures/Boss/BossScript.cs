using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Enemy
{
    [Serializable]
    public struct AttackPattern
    {
        public BulletAttack[] bulletAttacks;
    }
    
    [Tooltip("List of different combinations of Bullet Attacks.")]
    public List<AttackPattern> AttackPatterns = new List<AttackPattern>();

    private Dictionary<BulletAttack.FireDirection, GameObject> Emitters;
    private TaskManager taskManager = new TaskManager();
    

    void Start()
    {
        var tester = new Attack(this, AttackPatterns[0].bulletAttacks);
        // taskManager.Do(tester);
    }

    void Update()
    {
        taskManager.Update();
    }

    private class Attack : Task
    {
        private BossScript boss;
        private BulletAttack[] attacks;

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
            foreach (var emitter in boss.Emitters)
            {
                emitter.Value.transform.rotation = Quaternion.identity;
            }
        }
        
        internal override void Update()
        {
            //If we've run out of bullet patterns to fire, the attack is done
            if (currentAttack >= attacks.Length) 
                SetStatus(TaskStatus.Success);

            if (FireBulletAttack(attacks[currentAttack]))
                currentAttack++;

        }

        protected override void OnSuccess()
        {
            
        }

        protected override void OnFail()
        {
            
        }

        /// <summary>
        /// Takes a BulletAttack object and fires the attack according to them specific specifications
        /// </summary>
        private bool FireBulletAttack(BulletAttack attack)
        {
            if (duration >= attack.AttackDuration)
            {
                return true;
            }

            foreach (var pattern in attack.Patterns)
            {
                if (duration % pattern.FireRate == 0)
                {
                    Shoot(pattern.Direction, pattern.Angle, pattern.Offset);
                }
            }
            

            duration += Time.deltaTime;
            return false;
        }

        private void Shoot(BulletAttack.FireDirection direction, float angle, float offset)
        {
            
        }
        
        
    }
}
