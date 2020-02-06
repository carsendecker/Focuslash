using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int Damage;

    private Collider2D atkCol;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        atkCol.enabled = true;
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        atkCol.enabled = false;
    }
}
