/*
Author: Christian Mullins
Data: 02/02/2021
Summary: Basic mouse movements to look around in-game as a player and interact with objects.
*/
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCamera : MonoBehaviour {
    //public vars
    public float speed = 3f;
    public GameObject interactingWith;

    [Header("Look values for Camera Movement")]
    public float lookSensitivity = 5f;
    public float upperBoundary = 290f;  // lower = lower boundary
    public float lowerBoundary = 60f;   // lower = higher boundary
    public float maxZoomVal = 20f;      // lower = more zoom-in
    //private vars
    private CharacterController controller;
    private Camera cam;
    private Transform camTrans;

    void Start() {
        //intialize values
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        camTrans = cam.transform;
        interactingWith = null;
        //set up mouse for FPS view
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        controller.Move(GatherMoveInput() * speed);

        //gather look input appropriately
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"), Space.Self);
        float yInput = -Input.GetAxis("Mouse Y") * lookSensitivity;
        //check if this point of looking rotation is valid
        print(camTrans.localEulerAngles.x);
        if (yInput + camTrans.localEulerAngles.x < lowerBoundary 
            || yInput + camTrans.localEulerAngles.x > upperBoundary) {
            camTrans.Rotate(Vector3.right * yInput, Space.Self);
        } //end of gathering inputs

        //interact with objects
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit hit;
            //TO DO
                //CHECK IF THIS RAYCAST IS CORRECT
            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 5f)) {
                if (hit.transform.tag == "Interactable") {
                    //hit.GetComponenet<Interactable>().interactWith;
                }
            }
        }

        //zoom in for RMB
        if (Input.GetMouseButtonDown(1)) {
            StartCoroutine("ZoomIn");
        }
        else if (Input.GetMouseButtonUp(1) && cam.fieldOfView < 60) {
            StopCoroutine("ZoomIn");
            StartCoroutine("ZoomOut");
        }
    }

    /// <summary>
    /// Grabs inputs from WASD and outputs them as a Vector3.
    /// (Deprecate when official player movement implemented)
    /// </summary>
    /// <returns>Resultant Vector from WASD</returns>
    private Vector3 GatherMoveInput() {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        return new Vector3(xAxis, 0f, zAxis);
    }
    
    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom camera in.
    /// </summary>
    private IEnumerator ZoomIn() {
        //FOV starts at 60, ends at 20
        while (cam.fieldOfView > maxZoomVal) {
            --cam.fieldOfView;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return null;
    }

    /// <summary>
    /// Takes the current Main camera and manipulates the Field of View to zoom the camera back out.
    /// </summary>
    private IEnumerator ZoomOut() {
        //FOV starts at 20, ends at 60
        while (cam.fieldOfView < 60) {
            ++cam.fieldOfView;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
