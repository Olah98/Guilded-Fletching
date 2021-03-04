/*
Author: Christian Mullins
Date: 02/16/2021
Summary: Used only for testing the ranged enemy shooting system.
        NOT FOR GAME IMPLEMENTATION
*/
using UnityEngine;

public class EnemyAITester : MonoBehaviour {
    public GameObject testGO;
    public Camera testCam;

    private void Start() {
        // get the test camera and the enemy to test
        testGO = GameObject.FindGameObjectWithTag("Enemy");
        testCam = GameObject.Find("SecondCam").GetComponent<Camera>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3? testPos = GetPositionFromRaycast();

            if (testPos != null) {
                Debug.DrawLine(testGO.transform.position, (Vector3)testPos, Color.green, 5f);
                testGO.GetComponent<ArcherEnemy>().ShootAtTest((Vector3)testPos);
            }
        }
    }

    public Vector3? GetPositionFromRaycast() {
        RaycastHit hit;
        Ray ray = testCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            return hit.point;
        return null;
    }
}
