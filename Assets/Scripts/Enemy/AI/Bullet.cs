using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public GameObject Marker;
    public Vector2 TargetPosition;
    public Vector2 Direction;
    public bool MarkerInitialised = false;
    void Start()
    {
        //var newRot = new Quaternion(
        //    transform.rotation.x,
        //    transform.rotation.y,
        //    transform.rotation.z + Random.Range(-10, 10),
        //    transform.rotation.w);

        //transform.rotation = newRot;
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * Speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        //if (!MarkerInitialised && Marker != null)
        //{            
        //    Instantiate(Marker, TargetPosition, transform.rotation);
        //}
        //Debug.DrawLine(TargetPosition, transform.position);
        // transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed * Time.deltaTime);
        
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
            Destroy(gameObject);
        }

        if (coll.transform.tag == EntityConstants.WALL_TAG)
        {
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

}
