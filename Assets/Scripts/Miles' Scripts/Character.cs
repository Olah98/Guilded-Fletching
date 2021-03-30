/*
Author: Miles Gomez & Christian Mullins
Date: 3/15/2021
Summary: Script containing values of the player, their movement, and camera
    manipulation in-game.
*/
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
    public Transform bowPosition;
    [Header("Arrows: Standard, Bramble, Warp, Airburst")]
    public GameObject[] arrowPrefabs;

    [Header ("Movement")]
    public float speed;
    public float maxSpeed;
    public float jumpPower;
    public float jumpCD;
    public float fallMod;
    public float coyoteJump;
    public bool isClimbing;

    private bool canJump;
    private float horizontalInput;
    private float verticalInput;
    private float gravity = 9.8f;
    private Vector3 velocity;

    private Transform camEuler;
    private SavedData currentData;
    private List<GameObject> StandardArrowTracker = new List<GameObject>(); //Added by Warren
    private List<GameObject> BrambleArrowTracker = new List<GameObject>(); //Added by Warren

    [Range(0.1f, 0.9f)]
    public float minCrouchHeight = 0.5f;
    private float _coyoteJumpTime;

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
    public Slider chargeSlider;//Added by Warren

    /* BEGIN FIRSTPERSONCAMERA VARIABLES */
    #region Camera_Variables
    [Header("Camera Variables")]
    public Text interactionHintText;
    [Header("Look values for Camera Movement")]
    [Range(5f, 15f)]
    [Tooltip("This value is not the same as player mouseSensitivity.")]
    public float lookSpeed = 10f;
    [Tooltip("Lower value = lower look boundary")]
    public float upperBoundary = 290f;
    [Tooltip("Lower value = higher look boudary")]
    public float lowerBoundary = 60f;
    [Range(LOWER_ZOOM_BOUNDARY, UPPER_ZOOM_BOUNDARY)]
    [Tooltip("Length in which the player can zoom-in.")]
    public float maxZoomVal = 40f;

    private Camera _cam;
    private float _clampedMaxZoom { get {
        return Mathf.Clamp(s_baseFOV + maxZoomVal, 0f, 200f);
    } }
    // store option vars
    private float s_baseFOV;
    private float s_mouseSensitivity;
    // consts
    private const float LOWER_ZOOM_BOUNDARY = 20f;
    private const float UPPER_ZOOM_BOUNDARY = 60f;
    #endregion
    /* END FIRSTPERSONCAMERA VARIABLES */

    [Header("Testing")]
    public bool timedJump;
    public float upTime;

    private float _jumpTime = 0f;
    private bool _canJump;
    private Vector3 _velocity;
    private Quiver _myQuiver;
    private bool _isCrouching;
    private SavedData _currentData;
    private PlayerAnimationController _pAnimController;
    private bool _runOnce;

    // delegates
    public bool      isCrouching    { get { return _isCrouching; } }
    public SavedData getCurrentData { get { return currentData; } }
    public Quiver    getMyQuiver    { get { return _myQuiver;    } }
    public int       getMyArrowType { get { return _myQuiver.GetArrowType(); } }

    private void Start()
    {
        // intialize camera based values
        _cam = Camera.main;
        _isCrouching = false;
        interactionHintText.enabled = false;
        s_baseFOV = getCurrentData?.baseFOV ?? 60f;
        _cam.fieldOfView = s_baseFOV;
        // set up mouse for FPS view
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // end camera initialization
        isClimbing = false;
        cc = gameObject.GetComponent<CharacterController>();
        _pAnimController = GetComponent<PlayerAnimationController>();

        if (SaveManager.instance.activeSave.respawnPos!=null &&
            SaveManager.instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
        {
            cc.enabled = false;
            transform.position = SaveManager.instance.activeSave.respawnPos;
            cc.enabled = true;
        }
        // initialize quiver (StandardArrow equipped first)
        // initialize values if new game, else grab existing
        // if data comes back null (it shouldn't), create new instance
        _myQuiver = GetComponent<Quiver>();

        //LevelManager starter = GameObject.FindWithTag("Save Manager").GetComponent<LevelManager>();
        //if (starter != null)
        //{
        //    _myQuiver.Load(starter.loadoutName);
        //}


        _currentData = SavedData.GetDataStoredAt(SavedData.currentSaveSlot)
                        ?? new SavedData();
        UpdateCharacterToSaveData(_currentData);
    }

    private void Update()
    {
        
        if (!_runOnce)
        {
            _runOnce = true;
            LevelManager starter = GameObject.FindWithTag("Save Manager").GetComponent<LevelManager>();
            if (starter != null)
            {
                _myQuiver.Load(starter.loadoutName);
                _myQuiver.EquipType(0);//loads standard arrow by default
            }
            if (SaveManager.instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
            {
                _myQuiver.EquipType(SaveManager.instance.activeSave.equippedType);
                /*_myQuiver.ReplaceRecords(SaveManager.instance.activeSave.recordStandard,
                    SaveManager.instance.activeSave.recordBramble,
                    SaveManager.instance.activeSave.recordWarp,
                    SaveManager.instance.activeSave.recordAirburst);*/
                //_myQuiver.ReplaceLoadout(SaveManager.instance.activeSave.loadoutSaved);
                Debug.Log("Previous quiver recovered");
            }
        }

        // begin camera based updates
        // interact with objects
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 3.5f)) {
            if (hit.transform.tag == "Interactable"
                && hit.transform.GetComponent<Switch>().isInteractable) {
                if (Input.GetKeyDown(KeyCode.E))
                    InteractWithObject(hit.transform.gameObject);
                // display hint only under this condition
                interactionHintText.enabled = true;
            }
            else interactionHintText.enabled = false;
        }
        else interactionHintText.enabled = false;

        // zoom in/out using RMB
        if (Input.GetMouseButtonDown(1)) {
            StartCoroutine("ZoomIn");
        }
        else if (Input.GetMouseButtonUp(1) && _cam.fieldOfView < s_baseFOV) {
            StopCoroutine("ZoomIn");
            StartCoroutine("ZoomOut");
        }
        // end camera based updates

        #if UNITY_EDITOR
        //Used for testing, remove at a later date.
        if (Input.GetKeyDown(KeyCode.U)) Respawn();
        #endif
        //Checks if dead and respawns.
        if (dead) Respawn();

        cc.stepOffset = (_canJump) ? 0.2f : 0.0f;

        chargeText.text = "charge: " + Mathf.Floor(attackCharge);
        chargeSlider.GetComponent<Slider>().value = attackCharge;//Added by Warren

        // if the player is climbing, movement will be handled by Climber.cs
        if (isClimbing) return;

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

        bool isJumpPressed = Input.GetButton("Jump");
        if ((isJumpPressed && _canJump) || (isJumpPressed && _coyoteJumpTime > 0f))
        {
            if (!_canJump && _coyoteJumpTime > 0f)
            {
                _velocity.y = 0;
                Jump();
            }
            else
            {
                Jump();
            }
            //Jumps on input
        }
        else if (isJumpPressed && _canJump)
        {
            _velocity.y = 0;
        }
    }

    private void FixedUpdate()
    {
        // gather look input appropriately
        float xInput =  Input.GetAxis("Mouse X") * lookSpeed * s_mouseSensitivity;
        float yInput = -Input.GetAxis("Mouse Y") * lookSpeed * s_mouseSensitivity;
        Vector3 lookRot = new Vector3 (0f, xInput, 0f);
        // check if this point of looking rotation is valid
        if (yInput + _cam.transform.localEulerAngles.x < lowerBoundary
            || yInput + _cam.transform.localEulerAngles.x > upperBoundary) {
            // up and down looking (must be in local)
            _cam.transform.Rotate(Vector3.right * yInput, Space.Self);
        }
        //left to right looking (must be in world space)
        transform.Rotate(Vector3.up * xInput, Space.World);
        // all look based movement above

        // if the player is climbing, movement will be handled by Climber.cs
        if (isClimbing) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //Detects WASD movement or Jump

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        if (!dead && cc.enabled == true)
        {
            if (attackCharge == 0) {
                if (move != Vector3.zero)
                    _pAnimController.SetAnimation(AnimState.Walking, true);
                else
                    _pAnimController.SetAnimation(AnimState.Idle, true);
            }
            else if (attackCharge == 100) {
                _pAnimController.SetAnimation(AnimState.FullyDrawn, true);
            }
            cc.Move(move * speed * Time.deltaTime);
        }
        //Moves player when WASD is pressed

        if (_jumpTime <= 0)
        {
            if (!_canJump)
            {
                _velocity.y += Physics.gravity.y * Time.deltaTime * (1f + fallMod);
                            _pAnimController.SetAnimation(AnimState.Jumping, true);
            }
            if (_velocity.y < Physics.gravity.y)
            {
                _velocity.y = Physics.gravity.y;
            }
        }

        //responsible for Y axis movement
        if (cc.enabled == true)
        {
            cc.Move(_velocity * Time.deltaTime);
        }
        //Testing new jump system
        if (_jumpTime > 0 && timedJump == true)
        {
            _jumpTime -= Time.deltaTime;
        }

        _Crouching(Input.GetKey(KeyCode.LeftShift));

        if (Input.GetButton("Fire1"))
        {
            if (attackCD <= 0 && !dead)
            {
                if (attackCharge < 100)
                {
                    float drawMultiplier = arrowPrefabs[_myQuiver.GetArrowType()]
                                            .GetComponent<BaseArrow>().drawSpeed;
                    attackCharge += 40 * drawMultiplier * Time.fixedDeltaTime;
                    //builds attackcharge as long as you hold the mouse button down.
                    _pAnimController.SetAnimation(AnimState.DrawingArrow, true);
                }
                if (attackCharge > 100)
                {
                    attackCharge = 100; //Added by Warren for rounding
                    _pAnimController.SetAnimation(AnimState.FullyDrawn, true);
                }
            }

        }
        else if (attackCharge > 0)
        {
            if (attackCD <= 0 && !dead)
            {
                // get currently equipped arrow and increment record
                GameObject arrowEquipped = arrowPrefabs[_myQuiver.GetArrowType()];
                _myQuiver.Fire();
                //Checks that attack is off CD, shoots upon letting go of the mouse button
              // commented to merge this line and below   Fire(attackCharge, arrowEquipped, _cam.transform, bowPosition);
              //  _pAnimController.SetAnimation(AnimState.Shooting, true);
                //Expanded Fire to include arrow type
                GameObject projectile;
                projectile = Fire(attackCharge, arrowEquipped, _cam.transform, bowPosition);
                attackCD = 1;
                //Attack cd set  back to 1 second
                attackCharge = 0;
                //Bow Chare set back to 0


                //added by Warren
                if (_myQuiver.GetArrowType() == 0)
                {
                    TrackArrow(projectile, StandardArrowTracker);
                } else if (_myQuiver.GetArrowType() == 1)
                {
                    TrackArrow(projectile, BrambleArrowTracker);
                }
            }
        }
    }

    #region CameraFunctions
    /// <summary>
    /// Public function for settings to be updated to player defined options.
    /// </summary>
    /// <param name="data">SavedData that will update settings.</param>
    public void SetOptionVals(in OptionsData data)
    {
        s_baseFOV = data.baseFOV;
        _cam.fieldOfView = s_baseFOV;
        s_mouseSensitivity = data.mouseSensitivity;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to
    /// zoom the camera in.
    /// </summary>
    private IEnumerator ZoomIn()
    {
        // FOV starts at s_baseFOV, ends at s_baseFOV - maxZoomVal
        while (_cam.fieldOfView > s_baseFOV - maxZoomVal)
        {
            --_cam.fieldOfView;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return null;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom
    ///  the camera back out.
    /// </summary>
    private IEnumerator ZoomOut()
    {
        // FOV starts at s_baseFOV - maxZoomVal, ends at s_baseFOV
        while (_cam.fieldOfView < s_baseFOV)
        {
            ++_cam.fieldOfView;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    /// <summary>
    /// Determine what kind of object the player is interacting with and
    /// manipulate it accordingly.
    /// </summary>
    /// <param name="interactingWith">GameObject the player's interacting with.</param>
    private void InteractWithObject(GameObject interactingWith)
    {
        // handle if Switch
        if (interactingWith.TryGetComponent<Switch>(out Switch s)
            && s.isInteractable)
        {
            s.HitSwitch();
        }
        else if(interactingWith.TryGetComponent<ThankYou>(out ThankYou t))
        {
            t.load();
        }
        print("Interacting with: " + interactingWith.name);
        // to prevent "reinteraction"
        interactingWith.tag = "Untagged";
    }
    #endregion

    public void Jump()
    {
        if ((jumpCD <=0 && !dead) || (_coyoteJumpTime > 0 && !dead))
        {
            _canJump = false;
            //Adds force to jum pwith
            if (coyoteJump > 0)
            {
                _velocity.y = Mathf.Sqrt(jumpPower * -Physics.gravity.y * 1.2f);
            }
            else
            {
                _velocity.y = Mathf.Sqrt(jumpPower * -Physics.gravity.y);
            }
            jumpCD = .5f;
            _coyoteJumpTime = 0f;

            //testing New jump movement
            if (timedJump)
            {
                _jumpTime += upTime;
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

    public void GroundCheck()
    {
        //Checks if the player is on the ground and sets _canJump to true, if player is not on the ground, then it is false
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit, 1.3f))
        {
            _coyoteJumpTime = coyoteJump;
            _canJump = true;
        }
        else
        {
            _coyoteJumpTime -= Time.deltaTime * 1;
            _canJump = false;
        }
    }

    public void RoofCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, 1f, 0f), out hit, 1.2f))
        {
            if (_velocity.y > 0f)
            {
                _velocity.y = 0f;
            }
        }
    }

    public GameObject Fire(float attackCharge, GameObject arrow, Transform cam, Transform bowPosition)
    {
        GameObject projectile;
        projectile = Instantiate(arrow, bowPosition.transform.position, cam.transform.rotation);
        //creates force
        projectile.GetComponent<Rigidbody>().AddForce(cam.forward * attackCharge * 20f);
        //grants projectile force based on time spent charging attack
        return projectile; // added by Warren
    }

    //FUNCTIONS BELOW IN CLASS ARE WRITTEN BY CHRISTIAN
    /// <summary>
    /// Move the player with the motion of a platform.
    /// </summary>
    /// <param name="hit">Platform collider that the player hit.</param>
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.transform.tag == "Stoppable" ) {
            transform.parent = hit.transform;

        }
        else
            transform.parent = null;
    }

    /// <summary>
    /// Public function that removes health while checking for if the
    /// player has died.
    /// </summary>
    /// <param name="damage">Amount to damage player.</param>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 1)
        {
            // implement player death
            currentHp = 0;
            dead = true;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public SavedData UpdateAndGetSaveData()
    {
        _currentData.playerHealth = currentHp;
        _currentData.s_Quiver = new SerializableQuiver(_myQuiver);
        // future implementations will handle checkpoint system
        return _currentData;
    }

    /// <summary>
    /// Take argument data and overwrite Character's save data.
    /// </summary>
    /// <param name="data">Data that will be stored</param>
    /// <returns>Updated save file</returns>
    public void UpdateCharacterToSaveData(in SavedData data)
    {
        _currentData = data;
        currentHp = data.playerHealth;
        _myQuiver.CopySerializedQuiver(data.s_Quiver);
        var options = SavedData.GetStoredOptionsAt(SavedData.currentSaveSlot);
        // get saved data's stored options, then apply to scene
        SavedData.SetOptionsInScene(options);
    }

    //Written by Warren
    public void TrackArrow(GameObject projectile, List<GameObject> tracker)
    {
        tracker.Insert(0, projectile);
        if (tracker.Count > 2)
        {
            if (tracker[2] != null)
            {
                Destroy(tracker[2]);
                tracker.RemoveAt(2);
            }
        }
    }

    /// <summary>
    /// Called from FixedUpdate() this scales the player down to crouching size
    /// while pushing the player down the prevent possible floating.
    /// </summary>
    /// <param name="action">In parameter to enable or disable crouching</param>
    private void _Crouching(in bool action) {
        speed = (action) ? maxSpeed * 0.6f : maxSpeed;
        _isCrouching = action;
        float incrementor = Mathf.Lerp(minCrouchHeight,
                                       1.0f,
                                       transform.localScale.y);
        incrementor     = (action) ? -incrementor : incrementor;
        bool isBoundaryHit = (action) ? transform.localScale.y <= minCrouchHeight
                                      : transform.localScale.y >= 1.0f;
        if (!isBoundaryHit) {
            transform.localScale += new Vector3(0f, incrementor, 0f) * Time.fixedDeltaTime;
            // prevent the player from "floating"
            cc.SimpleMove(new Vector3(0f, incrementor, 0f) * Time.fixedDeltaTime);
        }
    }
}
