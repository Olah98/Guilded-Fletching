/*
Author: Christian Mullins
Data: 02/02/2021
Summary: Basic mouse movements to look around in-game as a player and
        interact with objects.
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//[System.Obsolete] // deprecate later
public class FirstPersonCamera : MonoBehaviour {
    public Text interactionHintText;
    [Header("Look values for Camera Movement")]
    [Range(5f, 15f)]
    [Tooltip("This value is not the same as player mouseSensitivity.")]
    public float lookSpeed = 10f;       // adjust for 
    public float upperBoundary = 290f;  // lower = lower boundary
    public float lowerBoundary = 60f;   // lower = higher boundary
    [Range(LOWER_ZOOM_BOUNDARY, UPPER_ZOOM_BOUNDARY)]
    [Tooltip("Length in which the player can zoom-in.")]
    public float maxZoomVal = 40f;      // greater the val, greater the zoom

    private Camera _cam;
    private Character _character;
    private Transform _bodyTrans;
    private float _clampedMaxZoom { get { 
        return Mathf.Clamp(s_baseFOV + maxZoomVal, 0f, 200f);
    } }
    // store option vars
    private float s_baseFOV;
    private float s_mouseSensitivity;
    // consts
    private const float LOWER_ZOOM_BOUNDARY = 20f;
    private const float UPPER_ZOOM_BOUNDARY = 60f;

    private void Start() {
        _character = GetComponentInParent<Character>();
        _cam = GetComponent<Camera>();
        _bodyTrans = transform.parent;
        interactionHintText.enabled = false;
        s_baseFOV = _character.getCurrentData?.baseFOV ?? 60f; 
        _cam.fieldOfView = s_baseFOV;
        // set up mouse for FPS view
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // physics/movement
    private void FixedUpdate() {
        // gather look input appropriately
        float xInput =  Input.GetAxis("Mouse X") * lookSpeed * s_mouseSensitivity;
        float yInput = -Input.GetAxis("Mouse Y") * lookSpeed * s_mouseSensitivity;
        Vector3 lookRot = new Vector3 (0f, xInput, 0f);
        // check if this point of looking rotation is valid
        if (yInput + transform.localEulerAngles.x < lowerBoundary
            || yInput + transform.localEulerAngles.x > upperBoundary) {
            // up and down looking (must be in local)
            transform.Rotate(Vector3.right * yInput, Space.Self);
        }
        //left to right looking (must be in world space)
        transform.parent.Rotate(Vector3.up * xInput, Space.World);
    }

    // inputs
    private void Update() {
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
    }

    /// <summary>
    /// Public function for settings to be updated to player defined options.
    /// </summary>
    /// <param name="data">SavedData that will update settings.</param>
    public void SetOptionVals(in OptionsData data) {
        s_baseFOV = data.baseFOV;
        _cam.fieldOfView = s_baseFOV;
        s_mouseSensitivity = data.mouseSensitivity;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to
    /// zoom the camera in.
    /// </summary>
    private IEnumerator ZoomIn() {
        // FOV starts at s_baseFOV, ends at s_baseFOV - maxZoomVal
        while (_cam.fieldOfView > s_baseFOV - maxZoomVal) {
            --_cam.fieldOfView;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return null;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom
    ///  the camera back out.
    /// </summary>
    private IEnumerator ZoomOut() {
        // FOV starts at s_baseFOV - maxZoomVal, ends at s_baseFOV
        while (_cam.fieldOfView < s_baseFOV) {
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
    private void InteractWithObject(GameObject interactingWith) {
        // handle if Switch
        if (interactingWith.TryGetComponent<Switch>(out Switch s) 
            && s.isInteractable) {
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
}