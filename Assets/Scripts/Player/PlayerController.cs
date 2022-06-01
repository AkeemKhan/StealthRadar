using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    public PlayerMovement PlayerMovement;

    public GameObject MeleeAttack;
    public GameObject MeleeOffset;

    public float AttackCooldown;

    void Start ()
    {
        InitialisePlayer();
    }

    // Update is called once per frame
    void Update ()
    {
        if (AttackCooldown >= 0)
            AttackCooldown -= Time.deltaTime;
        if (AttackCooldown <= 0)
        {
            if (Input.GetAxis("Fire1") > 0 || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space))
            {
                Attack();
            }
        }
    }

    public void InitialisePlayer()
    {
        PlayerStatistics.ResetStats();
        PlayerObject = gameObject;
    }

    public bool CanAttack()
    {
        return AttackCooldown <= 0;
    }

    public void Attack()
    {
        if (!CanAttack())
            return;

        AttackCooldown = 0.6f;
        var meleeAttack = Instantiate(MeleeAttack, MeleeOffset.transform.position, transform.rotation);
        var script = meleeAttack.GetComponent<Melee>();
        script.Damage = 40;

        PlayerStatistics.Stamina -= 3;
        if (PlayerStatistics.Stamina < 0)
            PlayerStatistics.Stamina = 0;
    }
}
