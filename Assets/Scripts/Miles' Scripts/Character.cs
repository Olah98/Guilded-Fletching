using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Author: Miles Gomez
//Changes made 2/10/2021

public class Character : MonoBehaviour
{
    CharacterController cc;
    public GameObject cam;
    private Transform camEuler;
    public float speed;
    public float jumpPower;
    public float jumpCD;
    public float maxSpeed;
    public float attackCD;
    public float attackCharge;
    public GameObject arrow;
    public Transform bowPosition;
    public bool isClimbing;
    private bool canJump;
    private float horizontalInput;
    private float verticalInput;
    private float gravity = 9.8f;
    private Vector3 velocity;
    private bool dead;

    private RespawnCoordinator rc;


    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        cc = gameObject.GetComponent<CharacterController>();
        rc = GameObject.FindGameObjectWithTag("RC").GetComponent<RespawnCoordinator>();
        if (rc.lastCheckpoint!=null)
        {
            transform.position = rc.lastCheckpoint;
        }


        //Sets Character controller
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rc.lastCheckpoint);
        Debug.Log(cam.transform.eulerAngles.x);
        //transform.rotation = Quaternion.Euler( new Vector3(cam.transform.eulerAngles.x, 0f));
        if (canJump)
        {
            cc.stepOffset = 0.2f;
        }
        if (!canJump)
        {
            cc.stepOffset = 0;
        }


        // if the player is climbing, movement will be handled by Climber.cs
        if (!isClimbing) return;

        //Debug.Log(cc.velocity);

        bool isJumpPressed = Input.GetButton("Jump");
        GroundCheck();
        //Checks Ground and if jump input has been pressed


        if (attackCD > 0)
        {
            attackCD -= 1 * Time.deltaTime;
            //lowers attack cd
        }

        if (jumpCD > 0)
        {
            jumpCD -= 1 * Time.deltaTime;
            //lowers attack cd
        }

        if (isJumpPressed && canJump)
        {
            Jump();
            //Jumps on input
        }
        else if (isJumpPressed && canJump)
        {
            velocity.y = 0;
        }
    }

    private void FixedUpdate()
    {

        // if the player is climbing, movement will be handled by Climber.cs
        if (isClimbing) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Detects WASD movement or Jump

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        cc.Move(move * speed * Time.deltaTime);
        //Moves player when WASD is pressed


        if (!canJump)
        {
            //gravity
            velocity.y -= gravity * Time.deltaTime;
        }

        if (velocity.y < -9.8f)
        {
            velocity.y = -9.8f;
        }

        //responsible for Y axis movement
        cc.Move(velocity * Time.deltaTime);

        if (Input.GetButton("Fire1"))
        {
            if (attackCD <= 0)
            {
                if (attackCharge < 100)
                {
                    attackCharge += 40 * Time.deltaTime;
                    //builds attackcharge as long as you hold the mouse button down.
                }
            }
        }
        else if (attackCharge > 0)
        {
            if (attackCD <= 0)
            {
                //Checks that attack is off CD, shoots upon letting go of the mouse button
                Attack.Fire(attackCharge, arrow, cam.transform, bowPosition);
                attackCD = 1;
                //Attack cd set  back to 1 second
                attackCharge = 0;
                //Bow Chare set back to 0
            }
        }

    }


    public void Jump()
    {
        if (jumpCD <=0)
        {
            canJump = false;
            //Adds force to jum pwith
            velocity.y = Mathf.Sqrt(jumpPower * 2f * gravity);
            jumpCD = 1;
        }


    }

    public void Die()
    {
        dead = true;
    }

    public void Respawn()
    {

    }

    public void Interact()
    {
        //Used to interact with objects in a level
    }

    public void GroundCheck()
    {
        //Checks if the player is on the ground and sets canJump to true, if player is not on the ground, then it is false

        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, -2.5f, 0f), out hit, 1.4f))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }
}


public class Attack : MonoBehaviour
{
    public static void Fire(float attackCharge, GameObject arrow, Transform cam, Transform bowPosition)
    {
        GameObject projectile;
        projectile = Instantiate(arrow, bowPosition.transform.position, cam.transform.rotation);
        //creates force
        projectile.GetComponent<Rigidbody>().AddForce(cam.forward * attackCharge * 20f);
        //projectile.transform.rotation = Quaternion.LookRotation(projectile.GetComponent.velocity);
        //grants projectile force based on time spent charging attack
    }
}
