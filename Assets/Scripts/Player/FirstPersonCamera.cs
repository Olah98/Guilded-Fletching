/*
Author: Christian Mullins
Data: 02/02/2021
Summary: Basic mouse movements to look around in-game as a player and 
        interact with objects.
*/
using UnityEngine;
using System.Collections;

public class FirstPersonCamera : MonoBehaviour {
    [Header("Look values for Camera Movement")]
    [Range(1f, 10f)]
    public float lookSensitivity = 5f;
    public float upperBoundary = 290f;  // lower = lower boundary
    public float lowerBoundary = 60f;   // lower = higher boundary
    [Range(0f, 50f)]
    public float maxZoomVal = 20f;      // lower = more zoom-in
    private Camera cam;
    private Transform bodyTrans;

    void Start() {
        cam = GetComponent<Camera>();
        bodyTrans = transform.parent;
        // set up mouse for FPS view
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // physics/movement
    void FixedUpdate() {
        // gather look input appropriately
        float xInput =  Input.GetAxis("Mouse X") * lookSensitivity;
        float yInput = -Input.GetAxis("Mouse Y") * lookSensitivity;
        Vector3 lookRot = new Vector3 (0f, xInput, 0f);
        // check if this point of looking rotation is valid
        if (yInput + transform.localEulerAngles.x < lowerBoundary 
            || yInput + transform.localEulerAngles.x > upperBoundary) {
            // up and down looking (must be in local)
            transform.Rotate(Vector3.right * yInput, Space.Self);
        }
        // left to right looking (must be in world space)
        transform.parent.Rotate(Vector3.up * xInput, Space.World);
    }

    // inputs
    void Update() {
        // interact with objects
        if (Input.GetKeyDown(KeyCode.E)) {
            if (Physics.Raycast(transform.position, transform.forward, 
                                out var hit, 3.5f)) {
                if (hit.transform.tag == "Interactable") {
                    InteractWithObject(hit.transform.gameObject);
                }
            }
        }
        // zoom in/out using RMB
        if (Input.GetMouseButtonDown(1)) {
            StartCoroutine("ZoomIn");
        }
        else if (Input.GetMouseButtonUp(1) && cam.fieldOfView < 60) {
            StopCoroutine("ZoomIn");
            StartCoroutine("ZoomOut");
        }
    }
    
    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to
    /// zoom camera in.
    /// </summary>
    private IEnumerator ZoomIn() {
        // FOV starts at 60, ends at 20
        while (cam.fieldOfView > maxZoomVal) {
            --cam.fieldOfView;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return null;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom
    ///  the camera back out.
    /// </summary>
    private IEnumerator ZoomOut() {
        // FOV starts at 20, ends at 60
        while (cam.fieldOfView < 60) {
            ++cam.fieldOfView;
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
        if (interactingWith.TryGetComponent<Switch>(out Switch s)) {
            s.HitSwitch();
        }
        // if this is a item pick-up
            // Do stuff
        // if this 
            // Do stuf
        print("Interacting with: " + interactingWith.name);
    }
}
