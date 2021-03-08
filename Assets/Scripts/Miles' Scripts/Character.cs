using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
//Author: Miles Gomez
//Changes made 3/4/2021

public class Character : MonoBehaviour
{
    [Header ("GameObjects")]
    CharacterController cc;
    public GameObject cam;
    public GameObject arrow;
    public Transform bowPosition;

    [Header ("Movement")]
    public float speed;
    public float maxSpeed;
    public float jumpPower;
    public float jumpCD;
    public float fallMod;
    public bool isClimbing;
    [Header("Arrows: Standard, Bramble, Warp, Airburst")]
    public GameObject[] arrowPrefabs;


    private bool canJump;
    private float horizontalInput;
    private float verticalInput;
    private float gravity = 9.8f;
    private Vector3 velocity;

    private Transform camEuler;
    private Quiver myQuiver;
    private SavedData currentData;


    [Header ("Attacking")]
    public float attackCD;
    public float chargeRate;
    public float attackCharge;

    [Header ("Health")]
    public int maxHp;
    public int currentHp;
    public bool dead = false;


    [Header ("UI Elements")]
    public Text chargeText;

    [Header("Testing")]
    public bool timedJump;
    public float upTime;
    private float jumpTime = 0f;



    // delegates
    public SavedData getCurrentData { get { return currentData; } }
    public Quiver    getMyQuiver    { get { return myQuiver;    } }

    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        cc = gameObject.GetComponent<CharacterController>();


        if (SaveManager.instance.activeSave.respawnPos!=null &&
            SaveManager.instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
        {
            cc.enabled = false;
            transform.position = SaveManager.instance.activeSave.respawnPos;
            cc.enabled = true;
        }


        //Sets Character controller

        // initialize quiver (StandardArrow equipped first)
        // initialize values if new game, else grab existing
        //          **** OR HAS THE EXISTING DATA ALREADY BEEN TAKEN CARE OF?****
        myQuiver = GetComponent<Quiver>();
        if (currentData == null)
            currentData = (SavedData)ScriptableObject.CreateInstance<SavedData>();
        UpdateCharacterToSaveData(currentData);
    }

    // Update is called once per frame
    void Update()
    {
        //Used for testing, remove at a later date.
        if (Input.GetKeyDown(KeyCode.U))
        {
            Respawn();
        }
        //Checks if dead and respawns.
        if (dead)
        {
            Respawn();
        }

        //Debug.Log(rc.lastCheckpoint);
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
        RoofCheck();
        //Checks To see if the player has touched a roof, will stop upwards movement.


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
        if (!dead && cc.enabled == true)
        {
            cc.Move(move * speed * Time.deltaTime);
        }
        //Moves player when WASD is pressed


        if (jumpTime <= 0)
        {
            if (!canJump)
            {
                //gravity
                velocity.y -= gravity * Time.deltaTime * (1f + fallMod);
            }

            if (velocity.y < -gravity)
            {
                velocity.y = -gravity;
            }
        }


        //responsible for Y axis movement
        if (cc.enabled == true)
        {
            cc.Move(velocity * Time.deltaTime);
        }

        //Testing new jump system
        if (jumpTime >0 && timedJump == true)
        {
            jumpTime -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            if (attackCD <= 0 && !dead)
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
            if (attackCD <= 0 && !dead)
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
        if (jumpCD <=0 && !dead)
        {
            canJump = false;
            //Adds force to jum pwith
            velocity.y = Mathf.Sqrt(jumpPower * gravity);
            jumpCD = .5f;

            //testing New jump movement
            if (timedJump)
            {
                jumpTime += upTime;
            }

        }


    }

    public void Respawn()
    {
        //Disables player movement.
        cc.enabled = false;
        StartCoroutine(RespawnCo());
    }

    public IEnumerator RespawnCo()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Interact()
    {
        if (!dead)
        {

        }
        //Used to interact with objects in a level
    }

    public void GroundCheck()
    {
        //Checks if the player is on the ground and sets canJump to true, if player is not on the ground, then it is false

        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit, 1.4f))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

    public void RoofCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, 1f, 0f), out hit, 1.2f))
        {
            if (velocity.y > 0f)
            {
                velocity.y = 0f;
            }
        }
    }




    //FUNCTIONS BELOW IN CLASS ARE WRITTEN BY CHRISTIAN
    /// <summary>
    /// Public function that removes health while checking for if the
    /// player has died.
    /// </summary>
    /// <param name="damage">Amount to damage player.</param>
    public void TakeDamage(int damage) {
        currentHp -= damage;
        if (currentHp < 1) {
            // implement player death
            currentHp = 0;
            dead = true;
        }
    }



    public void Damage(int damage)
    {
        currentHp -= damage;
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
        // update children
        GetComponentInChildren<FirstPersonCamera>().SetOptionVals(data);
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
