using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Enemy
{
    public List<BulletAttack> BulletAttacks = new List<BulletAttack>();

    private Dictionary<BulletAttack.FireDirection, GameObject> Emitters;
    private TaskManager taskManager = new TaskManager();
    

    void Start()
    {
        var tester = new DelegateTask(FireInit, delegate { return FireBulletAttack(BulletAttacks[0]); });
        taskManager.Do(tester);
    }

    void Update()
    {
        taskManager.Update();
    }

    /// <summary>
    /// Initialization for firing a bullet attack
    /// </summary>
    private void FireInit()
    {
        Debug.Log("Initialized!");
        currentAttack = 0;
        duration = 0;
    }

    /// <summary>
    /// Takes a BulletAttack object and fires the attack according to them specific specifications
    /// </summary>
    private float duration;
    private bool FireBulletAttack(BulletAttack attack)
    {
        if (duration >= attack.AttackDuration)
        {
            return true;
        }
        
        

        duration += Time.deltaTime;

        return false;
    }

    /// <summary>
    /// fires multiple attacks in succession
    /// </summary>
    int currentAttack = 0;
    private bool FireComboAttack(params BulletAttack[] attacks)
    {

        if (currentAttack == attacks.Length)
            return true;

        if (FireBulletAttack(attacks[currentAttack]))
            currentAttack++;

        return false;
    }

    private class CompoundAttack : Task
    {
        private BossScript boss;
        private BulletAttack attack;
        
        public CompoundAttack(BossScript boss, BulletAttack attack)
        {
            this.boss = boss;
            this.attack = attack;
        }
    }
}
