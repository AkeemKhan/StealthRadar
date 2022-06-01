using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public SpriteRenderer Sprite;
    public float AttackCooldown = 0.5f;
    public float Damage;
        
    void Start()
    {
        Sprite = transform.GetComponent<SpriteRenderer>();
        Destroy(gameObject, 0.2f);
    }

    void Update()
    {        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == EntityConstants.PLAYER_TAG)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
        if (collision.collider.tag == EntityConstants.ENEMY_TAG)
        {
            Debug.Log($"DAMAGE {Damage}");
            var ai = collision.collider.transform.GetComponent<EnemyAI>();
            if (ai is PatrolAI pAi)
            {
                pAi.PreventMove = 0.4f;
                pAi.SetAlert();
            }

            ai.DamageEnemy(Damage);            
        }
    }
}
