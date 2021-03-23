/*
Author: Christian Mullins
Date: 02/14/2021
Summary: Child of BaseEnemy class, specialized in range based attacks.
*/
using UnityEngine;

public class ArcherEnemy : BaseEnemy {
    [Header("Range based variables")]
    [Range(0, 100)]
    public int shotAccuracy;
    public GameObject arrowGO;
    public Transform bowTrans;
    private float retreatTimer;
    private Vector3 startPos;
    // for calculating player velocity
    private Vector3 playerVelocity;
    private Vector3 lastPlayerPos;

    protected override void Start() {
        base.Start();
        startPos = transform.position;
        playerVelocity = Vector3.zero;
        lastPlayerPos = _playerTrans.position;
    }

    protected override void FixedUpdate() {
        Vector3 moveDir = Vector3.zero;

        playerVelocity = GetCurrentPlayerVelocity();
        lastPlayerPos = _playerTrans.position;

        // check for aggro, increment timer, check for attack range
        isAggroed = IsPlayerInAggroRange();
        if (isAggroed) _attackTimer += Time.fixedDeltaTime;
        if (IsPlayerInAttackRange()) {
            if (_attackTimer >= attackFrequency) {
                Vector3 targetPos = GetNewShotPosition();
                moveDir = Vector3.Lerp(transform.position, targetPos, 0.5f) - transform.position;
                Vector3 shotDir = moveDir;
                moveDir.y = 0f;
                shotDir.y += AddShotArc(targetPos);
                transform.LookAt(moveDir + transform.position, transform.up);
                ShootAt(shotDir);
                return;
            }
        }
        else {
            // math based on: targetPos - currentPos = desiredDirection
            // move to player or retreat to startPos
            if (isAggroed)
                moveDir = _playerTrans.position - transform.position;
            // am I at the startPos?
            else if ((startPos - transform.position).magnitude > 1f)
                moveDir = startPos - transform.position;
        }
        // apply movement if necessary
        moveDir.y = 0f;
        transform.position += moveDir.normalized * speed * Time.fixedDeltaTime;
        // adjust rotation to always look forward
        transform.LookAt(moveDir + transform.position, transform.up);
    }

    /// <summary>
    /// Fire arrowGO at target position.
    /// </summary>
    /// <param name="target">Vector3 of position to shot.</param>
    protected override void ShootAt(Vector3 shotDir) {
        // create gameObject at bowPos
        GameObject shotGO = Instantiate(arrowGO, bowTrans.position, arrowGO.transform.rotation);
        shotGO.transform.parent = null;
        shotGO.transform.up = shotDir;
        // base power off of distance
        float power = Vector3.Distance(transform.position, _playerTrans.position) / 2.75f;
        // ForceMode.VelocityChange doesn't take rigidbody mass into account
        shotGO.GetComponent<Rigidbody>().AddForce(shotDir * power, ForceMode.VelocityChange);
        _attackTimer = 0f;
    }
    // for testing only (called from test scripts)
    public void ShootAtTest(Vector3 target) {
        Vector3 lookDir = (target - transform.position).normalized;
        lookDir.y = 0f;
        transform.LookAt(transform.position + lookDir.normalized, transform.up);
        Vector3 shotDir = Vector3.Lerp(transform.position, target, 0.5f)
                        - transform.position;
        shotDir.y += AddShotArc(target);
        ShootAt(shotDir);
    }

    /// <summary>
    /// Calculate and return position of possible hit or miss.
    /// </summary>
    /// <returns>Position to shoot at.</returns>
    protected Vector3 GetNewShotPosition() {
        Vector3 shootAt = _playerTrans.position;
        // apply player velocity (implemented in FixedUpdate)
        shootAt += playerVelocity * _playerTrans.GetComponent<Character>().speed;
        // RNG for accuracy
        if (Random.Range(0, 100) >= shotAccuracy) {
            // calculate miss (2 directions, and 2 severity values)
            int correctAxis = Random.Range(0, 3);
            float missSeverity = Random.Range(-6.5f, 6.5f);
            for (int i = 0; i < 3; ++i)
                if (i != correctAxis) shootAt[i] += missSeverity / -2f;
        }
        return shootAt;
    }

    /* PRIVATE FUNCTIONS FOR PROJECTILE SHOT CALCULATIONS */
    /// <summary>
    /// Create additional y-axis value by taking elevation change and horizontal
    /// distance into account.
    /// </summary>
    /// <param name="targetPos">Position of the enemy's target.</param>
    /// <returns>Additional y-axis padding.</returns>
    private float AddShotArc(Vector3 targetPos) {
        float arc = 0;
        // adjust arc for elevation change (negative value means downslope)
        float yDif = targetPos.y - transform.position.y;
        if (yDif > 5f) arc += 2.5f; 
        if (yDif < 0f) arc -= 0.75f;
        // adjust arc for horizontal distance
        targetPos.y = 0f;
        Vector3 myPos = Vector3.Scale(transform.position, new Vector3(1,0,1));
        if (Vector3.Distance(myPos, targetPos) > 25f) arc += 2.0f;

        return arc;
    }

    /// <summary>
    /// Use in Update() to track the player's velocity without utilizing
    /// CharacterController to track velocity (because it sucks).
    /// </summary>
    /// <returns>The current player velocity.</returns>
    private Vector3 GetCurrentPlayerVelocity() {
        Vector3 posChange = _playerTrans.position - lastPlayerPos;
        if (posChange.magnitude > 0f) {
            posChange = (_playerTrans.position - lastPlayerPos) / Time.fixedDeltaTime;
            // clamping value is relatively arbitrary, it just feels best when
            // taking player speed into account in GetNewShotPosition()
            return Vector3.ClampMagnitude(posChange, 0.35f);
        }
        return Vector3.zero;
    }

    // NOT IMPLEMENTED
    [System.Obsolete]
    /// <summary>
    /// Calculate the appropriate launch angle for a shot.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="maxHeight"></param>
    /// <returns>Launch angle in radians.</returns>
    private float GetLaunchAngle(Vector3 targetPos, float maxHeight) {
        float range = Mathf.Sqrt(Mathf.Pow(transform.position.x - targetPos.x, 2)
                    + Mathf.Pow(transform.position.z - targetPos.z, 2f));

        // NEGATIVE NUMBER POSSIBILE
        float offsetHeight = Mathf.Abs(targetPos.y - transform.position.y);
        float verticalSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * maxHeight);
        //print("adjusting vertical speed:" + 2f * -Physics.gravity.y * maxHeight);

        float travelTime = Mathf.Sqrt(2f * Mathf.Abs(maxHeight - offsetHeight)
                                            / -Physics.gravity.y) 
                         + Mathf.Sqrt(2f * maxHeight / -Physics.gravity.y);

        float horizontalSpeed = range / travelTime;
        float velocity = Mathf.Sqrt(Mathf.Pow(verticalSpeed, 2f) 
                       + Mathf.Pow(horizontalSpeed, 2f));
/*
        print("maxHeight: " + maxHeight);
        print("range: " + range);
        print("offsetHeight: " + offsetHeight);
        print("verticalSpeed: " + verticalSpeed);

        print("travelTime: " + travelTime); //bad
        print("horizontalSpeed: " + horizontalSpeed); //bad
        print("velocity: " + velocity); //bad
        print("-------------------------------");
*/
        float angleInRads = -Mathf.Atan2(verticalSpeed / velocity, 
                                         horizontalSpeed / velocity) + Mathf.PI;
        
        return angleInRads;
    }
}