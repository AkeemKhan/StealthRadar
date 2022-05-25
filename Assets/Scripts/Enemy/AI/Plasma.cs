using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public GameObject Marker;
    public GameObject Player;
    public Vector2 TargetPosition;
    public Vector2 Direction;
    public bool MarkerInitialised = false;
    void Start()
    {
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * Speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, Speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == EntityConstants.BULLET_TAG || coll.transform.tag == EntityConstants.ENEMY_TAG)
        {
            Physics2D.IgnoreCollision(coll.collider, GetComponent<Collider2D>());
        }

        if (coll.transform.tag == EntityConstants.PLAYER_TAG)
        {
            PlayerStatistics.DamagePlayer(Damage);
            PlayerStatistics.DamageStamina(1);

            coll.transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            coll.transform.GetComponent<Rigidbody2D>().freezeRotation = true;
            coll.transform.GetComponent<Rigidbody2D>().freezeRotation = false;

            Destroy(gameObject);
        }

        if (coll.transform.tag == EntityConstants.WALL_TAG)
        {
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

}
