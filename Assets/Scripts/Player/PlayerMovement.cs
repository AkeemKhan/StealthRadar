﻿using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public GameObject Player;
    public PlayerController PlayerController;

    public Sprite MovingSprite;
    public Sprite HollowSprite;

    public SpriteRenderer SpriteRenderer;

    public Vector3 ClickPosition;
    public Vector3 PreviousPosition;
    public Vector3 PreviousDirection;

    public FixedJoystick FixedJoystick;

    public bool IsMoving;
    public bool Sprinting;

    public bool ButtonSprinting;

    public float PlayerSpeed
    {
        get { return PlayerStatistics.MaxSpeed; }
        set { PlayerStatistics.MaxSpeed = value; }
    }

    // Use this for initialization
    void Start ()
    {
        ClickPosition = Player.transform.position;
        PreviousPosition = Player.transform.position;
        FixedJoystick = GameObject.Find("Fixed Joystick")?.GetComponent<FixedJoystick>();
        Player = transform.gameObject;
        SpriteRenderer = Player.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetMoveToPoint();
        Movement();        
    }

    public void SetMoveToPoint()
    {
        //float move = Input.GetAxis("Fire1");
        //if (move > 0)
        //{
        //    ClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    GetRotationPosition();
        //    RaycastHit hit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        ClickPosition.z = transform.position.z;
        //    }
        //}
        //

        Vector3 dir;
        var dirMod = transform.position;
        var movMod = transform.position;
        var dist = 0.01f;

        if (Input.GetKey(KeyCode.A))
        {
            dirMod.x += dist;
            movMod.x -= dist;
            dir = transform.position - dirMod;
            ClickPosition = movMod;
            GetRotationPosition(dir);
        }
        if (Input.GetKey(KeyCode.D))
        {                       
            dirMod.x -= dist;
            movMod.x += dist;
            dir = transform.position - dirMod;
            ClickPosition = movMod;
            GetRotationPosition(dir);
        }
        if (Input.GetKey(KeyCode.W))
        {
            dirMod.y -= dist;
            movMod.y += dist;
            dir = transform.position - dirMod;
            ClickPosition = movMod;
            GetRotationPosition(dir);
        }
        if (Input.GetKey(KeyCode.S))
        {            
            dirMod.y += dist;
            movMod.y -= dist;
            dir = transform.position - dirMod;
            ClickPosition = movMod;
            GetRotationPosition(dir);
        }

        if (FixedJoystick != null && FixedJoystick.Horizontal != 0)
        {
            Vector3 direction = new Vector2(transform.position.x, transform.position.y) + FixedJoystick.Direction;
            ClickPosition = direction;

            if (PreviousPosition != Player.transform.position)
            {
                PreviousDirection = FixedJoystick.Direction;
                GetRotationPosition(FixedJoystick.Direction);
            }
            else
            {
                GetRotationPosition(PreviousDirection);                
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprinting = true;
        }
        else
        {
            Sprinting = false;
        }
    }

    public void Movement()
    {
        var isSprinting = ButtonSprinting ? ButtonSprinting : Sprinting;

        if (PreviousPosition != Player.transform.position)
        {
            var newStamina = PlayerStatistics.Stamina - (isSprinting ? 10 * Time.deltaTime : Time.deltaTime);
            PlayerStatistics.Stamina = newStamina <= 0 ? 0 : newStamina;
            IsMoving = true;

            if (SpriteRenderer.sprite != MovingSprite)
                SpriteRenderer.sprite = MovingSprite;
        }
        else
        {
            var regen = (PlayerStatistics.MaxStamina * Time.deltaTime) / 10;
            //var newStamina = PlayerStatistics.Stamina + Time.deltaTime * 2f + (Time.deltaTime * PlayerStatistics.MaxStamina/100);
            var newStamina = PlayerStatistics.Stamina + regen;
            PlayerStatistics.Stamina = newStamina >= PlayerStatistics.MaxStamina ? PlayerStatistics.MaxStamina : newStamina;
            IsMoving = false;

            if (SpriteRenderer.sprite != HollowSprite)
                SpriteRenderer.sprite = HollowSprite;
        }

        PreviousPosition = Player.transform.position;
        var currentSpeed = PlayerStatistics.Stamina > 0 ? PlayerStatistics.Speed  * (isSprinting ? 1.8f : 1) : PlayerStatistics.Speed * 0.75f;

        transform.position = Vector2.MoveTowards(transform.position, ClickPosition, currentSpeed * Time.deltaTime);        
    }

    public void GetRotationPosition(Vector3? direction = null)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = direction.HasValue ? direction.Value : Input.mousePosition - pos;        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == EntityConstants.MELEE_TAG)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
