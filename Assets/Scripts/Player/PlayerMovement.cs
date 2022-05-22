using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public GameObject Player;
    public PlayerController PlayerController;
    public Vector3 ClickPosition;
    public Vector3 PreviousPosition;

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
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetMoveToPoint();
        Movement();        
    }

    public void SetMoveToPoint()
    {
        float move = Input.GetAxis("Fire1");
        if (move > 0)
        {
            ClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetRotationPosition();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                ClickPosition.z = transform.position.z;
            }

        }
    }

    public void Movement()
    {
        if (PreviousPosition != Player.transform.position)
        {
            var newStamina = PlayerStatistics.Stamina - Time.deltaTime;
            PlayerStatistics.Stamina = newStamina <= 0 ? 0 : newStamina;
        }
        else
        {
            var newStamina = PlayerStatistics.Stamina + Time.deltaTime * 2f;
            PlayerStatistics.Stamina = newStamina >= PlayerStatistics.MaxStamina ? PlayerStatistics.MaxStamina : newStamina;
        }

        PreviousPosition = Player.transform.position;
        var currentSpeed = PlayerStatistics.Stamina > 0 ? PlayerStatistics.Speed : PlayerStatistics.Speed * 0.75f;
        transform.position = Vector2.MoveTowards(transform.position, ClickPosition, currentSpeed * Time.deltaTime);        
    }

    public void GetRotationPosition()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }    
}
