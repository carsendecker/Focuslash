using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Enemy
{
    public List<BulletAttack> BulletAttacks = new List<BulletAttack>();
    
    
    private TaskManager taskManager = new TaskManager();
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private class BasicAttack : Task
    {
        private BossScript boss;
        private BulletAttack attack;
        
        public BasicAttack(BossScript boss, BulletAttack attack)
        {
            this.boss = boss;
            this.attack = attack;
        }
    }
}
