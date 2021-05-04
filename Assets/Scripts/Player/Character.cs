/*
Author: Miles Gomez & Christian Mullins
Date: 3/15/2021
Summary: Script containing values of the player, their movement, and camera
    manipulation in-game.
Referencing: DapperDino's UI Tutorial https://www.youtube.com/watch?v=Ikt5T-v2ZrM
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;//By Warren

//Author: Miles Gomez
//Changes made 3/4/2021

public class Character : MonoBehaviour
{
    //Bay for Constants
    //Rate at which the player heals. If the player takes 10 damage
    //and the modifier is 2, then the player recovers in 5 seconds.
    public const int DAMAGEMODIFIER = 3;

    [Header("GameObjects")]
    CharacterController cc;
    public Transform bowPosition;
    [Tooltip("Object is found as the child of the right hand.")]
    public Transform arrowPosition;
    [Header("Arrows: Standard, Bramble, Warp, Airburst")]
    public GameObject[] arrowPrefabs;

    [Header("Movement")]
    public float speed;
    public float maxSpeed;
    public float jumpPower;
    public float jumpCD;
    public float fallMod;
    public float coyoteJump;
    public bool isClimbing;

    private Vector3 _impact;
    private Transform camEuler;
    private List<GameObject> StandardArrowTracker = new List<GameObject>(); //Added by Warren
    private List<GameObject> BrambleArrowTracker = new List<GameObject>(); //Added by Warren

    [Range(0.1f, 0.9f)]
    public float minCrouchHeight = 0.5f;
    private float _coyoteJumpTime;

    [Header("Attacking")]
    public float attackCD;
    public float chargeRate;
    public float attackCharge;

    [Header("Health")]
    //public int maxHp;
    //public int currentHp;
    private int damageTaken;
    public bool dead = false;

    [Header("UI Elements")]
    public TMP_Text chargeText;
    public Slider chargeSlider;
    private GameObject _blackScreen;
    private Controls _controls;

    /* BEGIN FIRSTPERSONCAMERA VARIABLES */
    #region Camera_Variables
    [Header("Camera Variables")]
    public TMP_Text interactionHintText; //Change by Warren
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
    private GameObject _arrowInHand;
    private PlayerAnimationController _pAnimController;
    private Transform _originParent;
    private bool _runOnce;
    private bool _isJumpPressed;
    private bool _crouchReady;
    private bool _firePressed;
    private bool _zoomInReady;
    private bool _zoomOutReady;

    public int arrowAllowed = 5;

    // properties
    public bool      firePressed    => _firePressed;
    public bool      isCrouching    => _isCrouching;
    public SavedData getCurrentData => _currentData;
    public Quiver    getMyQuiver    => _myQuiver;
    public int       getMyArrowType => _myQuiver.GetArrowType();
    public Vector3   getVelocity    => _velocity;
    public bool      isSquashed     
        => Physics.Raycast(transform.position, Vector3.up, 1.2f);
    public float totalJumpPower
        => Mathf.Sqrt(jumpPower * -Physics.gravity.y * ((coyoteJump > 0) ? 1.2f : 1f));


    private void Awake()
    {
        _controls = new Controls(); //By Warren
        _controls.Player.Jump.performed += ctx => _isJumpPressed = true;
        _controls.Player.Crouch.started += ctx => _crouchReady = true;
        _controls.Player.Crouch.performed += ctx => _crouchReady = false;
        _controls.Player.Fire.started += ctx => _firePressed = true;
        _controls.Player.Fire.performed += ctx => _firePressed = false;
        _controls.Player.Zoom.started += ctx => _zoomInReady = true;
        _controls.Player.Zoom.performed += ctx => _zoomOutReady = true;
        _controls.Player.Interact.performed += ctx => Interact();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Start()
    {
        Destroy(GameObject.Find("Main Menu Music"));
        // intialize camera based values
        _cam = Camera.main;
        _originParent = transform.parent;
        _isCrouching = false;
        _pAnimController = GetComponent<PlayerAnimationController>();
        interactionHintText.enabled = false;
        s_baseFOV = getCurrentData?.baseFOV ?? 60f;
        _cam.fieldOfView = s_baseFOV;
        // set up mouse for FPS view
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // end camera initialization
        isClimbing = false;
        cc = GetComponent<CharacterController>();
        _blackScreen = GameObject.FindGameObjectWithTag("ScreenShift");

        if (SaveManager.instance.activeSave.respawnPos != null &&
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
        _SetArrowInHand(arrowPrefabs[getMyArrowType]); // set after quiver

        _currentData = SavedData.GetDataStoredAt(SavedData.currentSaveSlot) ?? new SavedData();
        UpdateCharacterToSaveData(_currentData);
        //grab options preferences and set them

    }

    private void Update()
    {
        if (!_runOnce)
        {
            _runOnce = true;
            LevelManager starter = GameObject.FindWithTag("Level Manager").GetComponent<LevelManager>();
            if (starter != null)
            {
                _myQuiver.Load(starter.loadoutName);
                _myQuiver.EquipType(0);//loads standard arrow by default
            }
            if (SaveManager.instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
            {
                _myQuiver.EquipType(SaveManager.instance.activeSave.equippedType);
                _myQuiver.ReplaceRecords(SaveManager.instance.activeSave.recordStandard,
                    SaveManager.instance.activeSave.recordBramble,
                    SaveManager.instance.activeSave.recordWarp,
                    SaveManager.instance.activeSave.recordAirburst);
                //_myQuiver.ReplaceLoadout(SaveManager.instance.activeSave.loadoutSaved);
                Debug.Log("Previous quiver recovered");
            }
        }

        // begin camera based updates
        // interact with objects
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, 3.5f))
        {
            if (hit.transform.tag == "Interactable")
            {
                //Modified by Warren to recognize when switches reset
                if ((hit.transform.GetComponent("Switch") as Switch) != null)
                {
                    if (hit.transform.GetComponent<Switch>().isInteractable)
                    {
                        // display hint only under this condition
                        interactionHintText.enabled = true;
                    }
                    else interactionHintText.enabled = false;
                }
                else
                {
                    // display hint only under this condition
                    interactionHintText.enabled = true;
                }
            }
            else interactionHintText.enabled = false;
        }
        else interactionHintText.enabled = false;

        // zoom in/out using RMB
        if (_zoomInReady)
        {
            StartCoroutine("_ZoomIn");
            _zoomInReady = false;
        }
        else if (_zoomOutReady && _cam.fieldOfView < s_baseFOV)
        {
            _zoomOutReady = false;
            StopCoroutine("_ZoomIn");
            StartCoroutine("_ZoomOut");
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

        if (_isJumpPressed)
        {
            _isJumpPressed = false;
            if (_canJump || (_coyoteJumpTime > 0f))
            {
                if (!_canJump && _coyoteJumpTime > 0f)
                {
                    _velocity.y = 0;
                }
                Jump();
                //Jumps on input
            }
            else if (_canJump)
            {
                _velocity.y = 0;
            }
        }

        if (_impact.magnitude > 0.2)
        {
            cc.Move(_impact * Time.deltaTime);
        }
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.deltaTime);
    } // end Update()

    private void FixedUpdate()
    {
        // gather look input appropriately
        //By Warren from Dapper Dino YT Tutorial Referenced above
        var lookInput = _controls.Player.Look.ReadValue<Vector2>();

        float xInput = lookInput.x * lookSpeed * s_mouseSensitivity;
        float yInput = -lookInput.y * lookSpeed * s_mouseSensitivity;
        Vector3 lookRot = new Vector3(0f, xInput, 0f);
        // check if this point of looking rotation is valid
        if (yInput + _cam.transform.localEulerAngles.x < lowerBoundary
            || yInput + _cam.transform.localEulerAngles.x > upperBoundary)
        {
            // up and down looking (must be in local)
            _cam.transform.Rotate(Vector3.right * yInput, Space.Self);
        }
        //left to right looking (must be in world space)
        transform.Rotate(Vector3.up * xInput, Space.World);

        //handle platform "squashing"
        if (Physics.Raycast(transform.position, Vector3.up, out var hit, 1.2f)
            && hit.transform.TryGetComponent<MovingPlatform>(out var mPlat) && !mPlat.isStopped)
        {
            StartCoroutine(mPlat.CheckForPlayerSquashing(this));
        }

        // all look based movement above
        // if the player is climbing, movement will be handled by Climber.cs
        if (isClimbing) return;

        //By Warren from Dapper Dino YT Tutorial Referenced above
        var movementInput = _controls.Player.Movement.ReadValue<Vector2>();
       
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        if (!dead && cc.enabled == true)
        {
            cc.Move(move.normalized * speed * Time.fixedDeltaTime);
        }
        //Moves player when WASD is pressed

        if (_jumpTime <= 0)
        {
            if (_velocity.y < Physics.gravity.y)
            {
                _velocity.y = Physics.gravity.y;
            }
            else if (!_canJump)
            {
                _velocity.y += Physics.gravity.y * Time.fixedDeltaTime * (1f + fallMod);
            }
        }
        
        //responsible for Y axis movement
        if (cc.enabled == true)
        {
            cc.Move(_velocity * Time.fixedDeltaTime);
        }
        //Testing new jump system
        if (_jumpTime > 0 && timedJump == true)
        {
            _jumpTime -= Time.fixedDeltaTime;
        }

        _Crouching(_crouchReady);

        // suspend all firing until damage animation is played
        if (_pAnimController.blockDrawInput)
        {
            //attackCD = 0f;
            //attackCharge = 0f;
            return;
        }

        // grab context
        if (_firePressed)
        {
            if (attackCD <= 0 && !dead)
            {
                if (attackCharge < 100)
                {
                    float drawMultiplier = arrowPrefabs[_myQuiver.GetArrowType()].GetComponent<BaseArrow>().drawSpeed;
                    attackCharge += 40 * drawMultiplier * Time.fixedDeltaTime;
                }
                if (attackCharge > 100)
                {
                    attackCharge = 100;
                }
            }
        }
        else if (!_firePressed && attackCharge > 0)
        {
            if (attackCD <= 0 && !dead)
            {
                // get currently equipped arrow and increment record
                GameObject arrowEquipped = arrowPrefabs[_myQuiver.GetArrowType()];
                _myQuiver.Fire();
                //Checks that attack is off CD, shoots upon letting go of the mouse button
                var projectile = Fire(attackCharge, arrowEquipped, _cam.transform, bowPosition);
                attackCD = 1;
                //Attack cd set  back to 1 second
                attackCharge = 0;
                //Bow Chare set back to 0

                //added by Warren
                if (_myQuiver.GetArrowType() == 0)
                {
                    TrackArrow(projectile, StandardArrowTracker);
                }
                else if (_myQuiver.GetArrowType() == 1)
                {
                    TrackArrow(projectile, BrambleArrowTracker);
                }
            }
        }

    } // end FixedUpdate()

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
    private IEnumerator _ZoomIn()
    {
        // FOV starts at s_baseFOV, ends at s_baseFOV - maxZoomVal
        while (_cam.fieldOfView > s_baseFOV - maxZoomVal)
        {
            --_cam.fieldOfView;
            yield return new WaitForSeconds(Time.deltaTime * 2f);
        }
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom
    ///  the camera back out.
    /// </summary>
    private IEnumerator _ZoomOut()
    {
        // FOV starts at s_baseFOV - maxZoomVal, ends at s_baseFOV
        while (_cam.fieldOfView < s_baseFOV)
        {
            ++_cam.fieldOfView;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// If the logic above means the interaction text is enabled then hitting
    /// the key will trigger the object in question
    /// </summary>
    private void Interact()
    {
        if (interactionHintText.enabled == true)
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, 3.5f))
            {
                if (hit.transform.tag == "Interactable")
                {
                    InteractWithObject(hit.transform.gameObject);
                }
            }
        }
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
        else if (interactingWith.TryGetComponent<ThankYou>(out ThankYou t))
        {
            //These are by Warren to initialize the Level End Screen
            SaveManager.instance.levelBeaten = true;
            SaveManager.instance.lastLoadout = _myQuiver.ReportLoadout();
            LevelManager timer = GameObject.FindWithTag("Level Manager").GetComponent<LevelManager>();
            if (timer != null)
            {
                SaveManager.instance.lastTime = timer.ReportTime();
            }
            SaveManager.instance.lastLocation = SceneManager.GetActiveScene().name;
            SaveManager.instance.nextLocation = t.level;
            t.load();
        }
        Debug.Log("Interacting with: " + interactingWith.name);
        // to prevent "reinteraction"
        interactingWith.tag = "Untagged";
    }
    #endregion

    public void Jump()
    {
        if ((jumpCD <= 0 && !dead) || (_coyoteJumpTime > 0 && !dead))
        {
            _canJump = false;
            //Adds force to jum pwith
            _velocity.y = totalJumpPower; //coyote jump handled in this property
            jumpCD = .5f;
            _coyoteJumpTime = 0f;

            //testing New jump movement
            if (timedJump)
            {
                _jumpTime += upTime;
            }
        }
    }

    //controls knockback effects
    public void AddImpact(Vector3 dir, float force)
    {
        //dir = dir.normalized;
        dir = new Vector3(dir.normalized.x, 0.5f, dir.normalized.z);
        _impact += dir * force;
    }

    public void Respawn()
    {
        //Disables player movement.
        cc.enabled = false;
        StartCoroutine(RespawnCo());
    }

    public IEnumerator RespawnCo()
    {
        //yield return new WaitForSeconds(.5f); //Shortened by Warren to help with screen fade
        yield return null;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(LoadSceneCo(SceneManager.GetActiveScene().name));//By Warren
    }

    public void GroundCheck()
    {
        //Checks if the player is on the ground and sets _canJump to true, if player is not on the ground, then it is false
        if (Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), 1.1f))
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

    public GameObject Fire(float attackCharge, GameObject arrow, Transform cam, Transform bowPosition)
    {
        GameObject projectile;
        var outQuat = Quaternion.LookRotation(bowPosition.position - transform.position, transform.up);
        projectile = Instantiate(arrow, bowPosition.transform.position, outQuat);
        //grants projectile force based on time spent charging attack
        projectile.GetComponent<Rigidbody>().AddForce(cam.forward * attackCharge * 20f);
        StartCoroutine(_HideArrowForShot());
        return projectile;
    }
    
    /// <summary>
    /// Move the player with the motion of a platform only if the player is
    /// above the platform.
    /// </summary>
    /// <param name="hit">Platform collider that the player hit.</param>
    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (hit.transform.tag == "Stoppable" && hit.point.y > hit.transform.position.y)
            transform.parent = hit.transform;
        else
            transform.parent = _originParent;
    }

    /// <summary>
    /// Public function that removes health while checking for if the
    /// player has died.
    /// </summary>
    /// <param name="damage">Amount to damage player.</param>
    public void TakeDamage(int damage)
    {
        if (damageTaken != 0)
        //print("TAKING DAMAGE");
        //currentHp -= damage;
        if (false) //currentHp < 1)
        {
            // implement player death
            damageTaken = 0;
            dead = true;
        }
        else
        {
            //Recovery time is damage / modifier in seconds
            damageTaken = Mathf.FloorToInt(damage/DAMAGEMODIFIER);
            StartCoroutine(RecoverHealth());
            if (_blackScreen != null)
            {
                _blackScreen.GetComponent<ScreenShift>().ToggleDamage();
            }
        }

        // Lock out the player from being able to shoot or charge
        // if arrow charge is in progress, reset for damage
        attackCharge = 0f;
        attackCD = 0f;
        //StartCoroutine(_PauseDrawing());
        _pAnimController.TriggerDamageAnim();
    }

    /// <summary>
    /// Construct save data based on current game conditions and return them.
    /// </summary>
    /// <returns>Up-to-date SavedData.</returns>
    public SavedData UpdateAndGetSaveData()
    {
        //_currentData.playerHealth = currentHp;
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
        //currentHp = data.playerHealth;
        _myQuiver.CopySerializedQuiver(data.s_Quiver);
        var options = SavedData.GetStoredOptionsAt(SavedData.currentSaveSlot);
        // get saved data's stored options, then apply to scene
        SavedData.SetOptionsInScene(options);
        SetOptionVals(options);
    }

    //Written by Warren
    public void TrackArrow(GameObject projectile, List<GameObject> tracker)
    {
        tracker.Insert(0, projectile);
        if (tracker.Count > arrowAllowed)
        {
            if (tracker[arrowAllowed] != null)
            {
                Destroy(tracker[arrowAllowed]);
                tracker.RemoveAt(arrowAllowed);
            }
        }
    }

    /// <summary>
    /// Called from FixedUpdate() this scales the player down to crouching size
    /// while pushing the player down the prevent possible floating.
    /// </summary>
    /// <param name="action">In parameter to enable or disable crouching</param>
    private void _Crouching(in bool action)
    {
        speed = (action) ? maxSpeed * 0.6f : maxSpeed;
        _isCrouching = action;
        float incrementor = Mathf.Lerp(minCrouchHeight, 1.0f, transform.localScale.y);
        incrementor        = (action) ? -incrementor : incrementor;
        bool isBoundaryHit = (action) ? transform.localScale.y <= minCrouchHeight
                                      : transform.localScale.y >= 1.0f;

        var ipt = bowPosition.GetComponent<IgnoreParentTransforms>();
        if (!isBoundaryHit)
        {
            ipt.SetParentInstance(true, action);
            Vector3 incrementDir = new Vector3(0f, incrementor, 0f);
            transform.localScale += incrementDir * Time.fixedDeltaTime;
            // prevent the player from "floating"
            cc.SimpleMove(incrementDir * Time.fixedDeltaTime);
        } else if (!action) {
            ipt.SetParentInstance(false, action);
        }
    }

    #region ArrowInHand_Handling
    /// <summary>
    /// Call _SetArrowInHand publically with some restrictions.
    /// </summary>
    /// <param name="index">Index of the prefab arrows array.</param>
    public void SetArrowInHandByIndex(int index)
    {
        _SetArrowInHand(arrowPrefabs[index]);
    }
    /// <summary>
    /// Set the visible arrow in the player's hand to the parameter
    /// GameObject given.
    /// </summary>
    /// <param name="setArrow">The object being shown in the player's hands.</param>
    private void _SetArrowInHand(GameObject setArrow)
    {
        // check if Warp arrow (GameObject is "built different")
        var warp = setArrow.GetComponent<WarpArrow>();
        if (warp == null)
        {
            arrowPosition.GetComponent<MeshFilter>().mesh
                = setArrow.GetComponent<MeshFilter>().sharedMesh;
            arrowPosition.GetComponent<MeshRenderer>().material
                = setArrow.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            // check for children, if so delete objects
            if (arrowPosition.transform.childCount > 0)
            {
                for (int i = 0; i < arrowPosition.transform.childCount; ++i)
                {
                    Destroy(arrowPosition.transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            var warpChildren = setArrow.GetComponentsInChildren<Transform>();
            arrowPosition.GetComponent<MeshFilter>().mesh = null;
            arrowPosition.GetComponent<MeshRenderer>().material = null;
            foreach (var c in warpChildren)
            {
                Instantiate(c.gameObject, arrowPosition.transform);
            }
        }
    }

    /// <summary>
    /// Hide arrow in-hand GameObject to wait until the player can fire again.
    /// </summary>
    private IEnumerator _HideArrowForShot()
    {
        arrowPosition.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f); // adjust later
        arrowPosition.gameObject.SetActive(true);
    }
    #endregion

    /* FUNCTIONS ADDED BY WARREN */
    /*
    * Delay - By Warren 
    * Calls a screen shift and waits for the change, if a target is available
    */
    public IEnumerator Delay()
    {
        if (_blackScreen != null)
        {
            _blackScreen.GetComponent<ScreenShift>().Change();
            yield return new WaitForSeconds(.5f);
        }
    }//Delay

    /*
    * Load Scene Co - By Warren 
    * Asks for a delay for a screen fade before loading the target level/menu 
    */
    public IEnumerator LoadSceneCo(string level)
    {
        yield return Delay();
        SceneManager.LoadScene(level);
    }//LoadSceneCo

   /* 
    * RecoverHealth - By Warren 
    * Recovers a point of damageTaken per second
    * Turns on and off warning lights
    */
    public IEnumerator RecoverHealth()
    {
        if (damageTaken > 0)
        {
            yield return new WaitForSeconds(1f);
            damageTaken--;
            StartCoroutine(RecoverHealth());
        } else
        {
            if (_blackScreen != null)
            {
                _blackScreen.GetComponent<ScreenShift>().ToggleDamage();
            }
        }
    }//RecoverHealth

}
