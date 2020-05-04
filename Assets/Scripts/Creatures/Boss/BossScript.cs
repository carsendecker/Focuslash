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
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Takes a BulletAttack object and fires the attack according to them specific specifications
    /// </summary>
    private bool FireBulletAttack(BulletAttack attack)
    {
        
        return false;
    }

    private void FireComboAttack(params BulletAttack[] attacks)
    {
        
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
