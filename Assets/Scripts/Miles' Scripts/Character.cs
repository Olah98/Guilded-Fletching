using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//Author: Miles Gomez
//Changes made 2/10/2021

public class Character : MonoBehaviour
{
    public GameObject cam;
    public int health;
    public float speed;
    public float jumpPower;
    public float jumpCD;
    public float maxSpeed;
    public float attackCD;
    public float attackCharge;
    public Transform bowPosition;
    public bool isClimbing;
    public Text chargeText;
    [Header("Arrows: Standard, Bramble, Warp, Airburst")]
    public GameObject[] arrowPrefabs;

    private CharacterController cc;
    private bool canJump;
    private float horizontalInput;
    private float verticalInput;
    private float gravity = 9.8f;
    private Vector3 velocity;
    private bool dead;
    private Transform camEuler;
    private RespawnCoordinator rc;
    private Quiver myQuiver;
    private SavedData currentData;

    // delegates
    public Quiver    getMyQuiver    { get { return myQuiver;    } }
    public SavedData getCurrentData { get { return currentData; } }
    
    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        cc = gameObject.GetComponent<CharacterController>();
      // rc = GameObject.FindGameObjectWithTag("RC").GetComponent<RespawnCoordinator>();
      // if (rc.lastCheckpoint!=null)
      // {
      //     transform.position = rc.lastCheckpoint;
      // }
        //Sets Character controller
        
        // initialize quiver (StandardArrow equipped first)
        myQuiver = GetComponent<Quiver>();
        currentData = (SavedData)ScriptableObject.CreateInstance(
                                                    typeof(SavedData));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rc.lastCheckpoint);
        //Debug.Log(cam.transform.eulerAngles.x);
        //transform.rotation = Quaternion.Euler( new Vector3(cam.transform.eulerAngles.x, 0f));
        if (canJump)
        {
            cc.stepOffset = 0.2f;
        }
        if (!canJump)
        {
            cc.stepOffset = 0;
        }

        chargeText.text = "charge: " + attackCharge;

        // if the player is climbing, movement will be handled by Climber.cs
        if (isClimbing) return;

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
                    float drawMultiplier = arrowPrefabs[myQuiver.GetArrowType()]
                                            .GetComponent<BaseArrow>().drawSpeed;
                    attackCharge += 40 * drawMultiplier * Time.fixedDeltaTime;
                    //builds attackcharge as long as you hold the mouse button down.
                }
            }
        }
        else if (attackCharge > 0)
        {
            if (attackCD <= 0)
            {
                // get currently equipped arrow and increment record
                GameObject arrowEquipped = arrowPrefabs[myQuiver.GetArrowType()];
                myQuiver.Fire();
                //Checks that attack is off CD, shoots upon letting go of the mouse button
                Attack.Fire(attackCharge, arrowEquipped, cam.transform, bowPosition);
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

    //FUNCTIONS BELOW IN CLASS ARE WRITTEN BY CHRISTIAN
    /// <summary>
    /// Public function that removes health while checking for if the
    /// player has died.
    /// </summary>
    /// <param name="damage">Amount to damage player.</param>
    public void TakeDamage(int damage) {
        health -= damage;
        if (health < 1) {
            // implement player death
            health = 0;
            dead = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public SavedData UpdateAndGetSaveData() {
        currentData.playerHealth = health;
        currentData.s_Quiver = new SerializableQuiver(myQuiver);
        // future implementations will handle checkpoint system
        //SavedData.StoreDataAtSlot(data, SavedData.currentSaveSlot);
        return currentData;
    }

    /// <summary>
    /// Take argument data and overwrite Character's save data.
    /// </summary>
    /// <param name="data">Data that will be stored</param>
    /// <returns>Updated save file</returns>
    public void UpdateCharacterToSaveData(in SavedData data) {
        currentData = data;
        health = data.playerHealth;
        myQuiver.CopySerializedQuiver(data.s_Quiver);
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
