using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Author: Miles Gomez
//Changes made 2/9/2021

public class Character : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float maxSpeed;
    public float attackCD;
    public float attackCharge;
    private Rigidbody rb;
    private bool canJump;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //Sets rbb
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isJumpPressed = Input.GetButton("Jump");
        //Detects WASD movement or Jump

        GroundCheck();
        Movement.Move(horizontalInput, verticalInput, speed, rb);

        if (isJumpPressed)
        {
            //Detects if the player has presssed spacebar
            Movement.Jump(jumpPower, canJump, rb);
        }

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

public class Movement : MonoBehaviour
{
    public static void Move(float horizontalInput, float verticalInput, float speed, Rigidbody rb)
    {
        //Moves player depending on what 
        rb.velocity = new Vector3(horizontalInput * speed, rb.velocity.y, verticalInput * speed);
       
    }

    public static void Jump(float jumpPower, bool canJump, Rigidbody rb)
    {
        //Checks if the player can jump, then if it's true, jumps based on jumpPower while maintaining momentum
        if (canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
        }
    }
}

public class Attack : MonoBehaviour
{

}
