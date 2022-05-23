using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public Vector2 TargetPosition;
    public Vector2 Direction;

    void Start()
    {

    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == EntityConstants.PLAYER_TAG)
        {
            PlayerStatistics.DamagePlayer(Damage);
            Destroy(gameObject);
        }

        if (coll.transform.tag == EntityConstants.WALL_TAG)
        {
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

}
