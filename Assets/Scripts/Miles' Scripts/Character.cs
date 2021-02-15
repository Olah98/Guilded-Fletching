using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Author: Miles Gomez
//Changes made 2/10/2021

public class Character : MonoBehaviour
{
    CharacterController cc;
    public float speed;
    public float jumpPower;
    public float maxSpeed;
    public float attackCD;
    public float attackCharge;
    public GameObject arrow;
    private bool canJump;
    public Transform bowPosition;
    private float horizontalInput;
    private float verticalInput;
    private float gravity = 9.8f;
    private Vector3 velocity;

    public Text charge;
    //Delete this later, used just for OBS
        
    // Start is called before the first frame update
    void Start()
    {
        cc = gameObject.GetComponent<CharacterController>();
        
        //Sets Character controller
    }

    // Update is called once per frame
    void Update()
    {
        charge.text = "Charge: " + attackCharge;
        bool isJumpPressed = Input.GetButton("Jump");
        GroundCheck();
        //Checks Ground and if jump input has been pressed
    
       
        if (attackCD > 0)
        {
            attackCD -= 1 * Time.deltaTime;
            //lowers attack cd
        }

        if (isJumpPressed && canJump)
        {
            Jump();
            //Jumps on input
        }

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

        if (Input.GetButtonUp("Fire1"))
        {
            if (attackCD <= 0)
            {
                //Checks that attack is off CD, shoots upon letting go of the mouse button
                Attack.Fire(attackCharge, arrow, transform, bowPosition);
                attackCD = 1;
                //Attack cd set  back to 1 second
                attackCharge = 0;
                //Bow Chare set back to 0
            }
        }
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Detects WASD movement or Jump

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        cc.Move(move * speed * Time.deltaTime);
        //Moves player when WASD is pressed

        //gravity
        velocity.y -= gravity * Time.deltaTime;
        //responsible for Y axis movement
        cc.Move(velocity * Time.deltaTime);
     
    }


    public void Jump()
    {
        //Adds force to jum pwith
        velocity.y = Mathf.Sqrt(jumpPower * 2f * gravity);

    }


    public void Interact()
    {
        //Used to interact with objects in a level
    }    

    public void GroundCheck()
    {
        //Checks if the player is on the ground and sets canJump to true, if player is not on the ground, then it is false
      
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit, 1.1f))
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
    public static void Fire(float attackCharge, GameObject arrow, Transform character, Transform bowPosition)
    {
        GameObject projectile;
        projectile = Instantiate(arrow, bowPosition.transform.position, character.transform.rotation);
        //creates force
        projectile.GetComponent<Rigidbody>().AddForce(character.forward * attackCharge*20f);

        //grants projectile force based on time spent charging attack
    }
}
