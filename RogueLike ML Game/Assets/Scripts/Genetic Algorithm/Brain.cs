using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public bool initialized = false;
    public GameObject bullet;
    public int numActiveBullets = 0;
    public DNA preferenceDNA;
    public DNA shootingDNA;
    public DNA movingDNA;
    public DNA strafingDNA;
    public float movementSpeed = 0.5f;
    public float timeAlive;
    public int damageDealt = 0;
    public float bounceDamp = 0.05f;
    public bool alive = true;
    public bool playerLooking = false;
    public Transform fireTransform;
    public Vector3 PlayerPosition
    {
        private get
        {
            return playerPosition;
        }
        set
        {
            playerStopped = value == playerPosition;

            if (!playerStopped) playerMovement = value - playerPosition;

            playerPosition = value;
        }
    }

    private readonly int DNALength = 32;
    private readonly int possibleEvents1 = 4;
    private readonly int possibleEvents2 = 3;
    private readonly int possibleEvents3 = 3;
    private readonly int maxDistance = 15;
    private readonly int maxTilt = 30;
    private readonly float tiltSpeed = 0.4f;

    #region
        //Basic Values
    private readonly int FORWARD = 1;
    private readonly int DIRECTLY = 0;
    private readonly int BACKWARD = -1;

    private readonly int I_STOPPED = 1;
    private readonly int P_LOOKING = 2;
    private readonly int P_STOPPED = 4;
    private readonly int I_SHOOT = 8;
    private readonly int P_CLOSE = 16;
    #endregion

    private Rigidbody rb;
    public Transform playerTransform;
    private Vector3 playerPosition = Vector3.zero;
    private Vector3 playerMovement = Vector3.zero;
    private float currentZTilt = 0;
    private float currentXTilt = 0;
    private float enemyHeight;
    private bool playerStopped = false;

    private readonly float distanceCheckIn = 0.75f;
    public float requiredDeltaDistance;
    private float currentDistanceCounter = 0;
    private Vector3 lastRecordedPosition;

    private readonly float shootingSpread = 1.5f;
    private float timeToShootAgain = 0;
    public float spinnerSpeedUp = 0.5f;

    private void FixedUpdate()
    {
        if (!alive || FindObjectOfType<Pause>().Paused) return;

        int parameters = 0;
        
        currentDistanceCounter += Time.fixedDeltaTime;
        timeToShootAgain += Time.deltaTime;

        if (currentDistanceCounter >= distanceCheckIn)
        {
            currentDistanceCounter = 0;

            float deltaDistance = Vector3.Distance(lastRecordedPosition, transform.position);
            lastRecordedPosition = transform.position;

            if (deltaDistance < requiredDeltaDistance)
                parameters += I_STOPPED;
        }

        if (playerLooking) parameters += P_LOOKING;
        playerLooking = false;

        if (playerStopped) parameters += P_STOPPED;
        
        if (timeToShootAgain < shootingSpread)
            parameters += I_SHOOT;

        if (playerTransform != null && Vector3.Distance(playerTransform.position, transform.position) > preferenceDNA.GetGene(parameters))
            parameters += P_CLOSE;

        int activeGene1 = shootingDNA?.GetGene(parameters) != null ? shootingDNA.GetGene(parameters) : -1;
        int activeGene2 = movingDNA?.GetGene(parameters) != null ? movingDNA.GetGene(parameters) : -1;
        int activeGene3 = strafingDNA?.GetGene(parameters) != null ? strafingDNA.GetGene(parameters) : -1;

        PlayerPosition = playerTransform.position;
        transform.LookAt(new Vector3(PlayerPosition.x, enemyHeight, PlayerPosition.z));
        Vector2 direction = Vector2.zero;

        switch (activeGene1)
        {
            case 0: break;  //Not shooting anywhere
            case 1: ShootAtPlayer(DIRECTLY);
                break;
            case 2: ShootAtPlayer(FORWARD);
                break;
            case 3: ShootAtPlayer(BACKWARD);
                break;
        }

        if (timeToShootAgain < shootingSpread || activeGene1 == 0)
        {
            switch (activeGene2)
            {
                case 0: break;  //Not moving forward or back
                case 1:
                    direction.y += FORWARD;
                    break;
                case 2:
                    direction.y += BACKWARD;
                    break;
            }

            switch (activeGene3)
            {
                case 0: break; //Not strafing left or right
                case 1:
                    direction.x += FORWARD;
                    break;
                case 2:
                    direction.x += BACKWARD;
                    break;
            }
        }

        Move(direction);
        if (direction.magnitude != 0) Movement(direction);
        
        timeAlive = PopulationManager.elapsed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string other = collision.gameObject.tag;

        if (other == "sword")
        {
            Died();
        }
    }

    public void Reset()
    {
        transform.position = FindObjectOfType<PopulationManager>().transform.position;
        timeAlive = 0;
        alive = true;
        GetComponent<Collider>().enabled = true;
        GetComponentInChildren<TrailRenderer>().enabled = true;

        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            rend.enabled = true;
    }

    /******************** Local Methods ********************/
    private void ShootAtPlayer(int direction)
    {
        SpinningBladeMod weapon = GetComponentInChildren<SpinningBladeMod>();

        if (timeToShootAgain < shootingSpread) {
            if (weapon.spinSpeed > 0) weapon.spinSpeed -= spinnerSpeedUp;
            return;
        }

        if (playerTransform != null)
        {
            Vector3 lookAt = PlayerPosition + playerMovement * direction;
            lookAt.y = 0.5f;
            fireTransform.LookAt(lookAt);
        }

        if (numActiveBullets < 10)
        {
            if (weapon.spinSpeed < RotorSpinning.spinSpeed) weapon.spinSpeed += spinnerSpeedUp;

            if (weapon.spinSpeed >= RotorSpinning.spinSpeed)
            {
                GameObject newBullet = Instantiate(bullet, fireTransform.position, fireTransform.rotation);
                newBullet.GetComponentInChildren<EnemyBulletCollision>().parent = this;
                newBullet.layer = LayerMask.NameToLayer("EnemyBullets");
                newBullet.GetComponentsInChildren<Transform>()[1].gameObject.layer = LayerMask.NameToLayer("EnemyBullets");
                numActiveBullets++;
                timeToShootAgain = 0;
            }
        }
    }

    private void Move(Vector2 direction)
    {
        //Setting up for floating that changes according to what it's floating over
        float groundHeight = 0;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, LayerMask.GetMask("Ground")))
        {
            groundHeight = hit.transform.position.y + hit.transform.lossyScale.y / 2;
            //Debug.DrawRay(transform.position, Vector3.down * 10, Color.green, 0.1f);
        }
        //else
        //    Debug.DrawRay(transform.position, Vector3.down * 10, Color.red, 0.1f);
        
        float forceFactor = 1f - ((transform.position.y - groundHeight) / enemyHeight);

        if (forceFactor > 0f)
        {
            Vector3 upLift = -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
            rb.AddForceAtPosition(upLift, transform.position);
            //Debug.Log("Floating... " + upLift);
        }

        //Actually Moving
        Vector3 movementVector = transform.TransformDirection(new Vector3(direction.x, 0, direction.y) * movementSpeed);
        movementVector.y = 0;
        Vector3 newPosition = transform.position + movementVector;
        //Debug.Log("position: " + newPosition);

        rb.MovePosition(newPosition);
    }

    private void Movement(Vector2 direction)
    {
        Vector3 newRotation = Vector3.zero;

        if (direction.x != 0)
        {
            newRotation.z = Mathf.Lerp(currentZTilt, maxTilt * -direction.x, tiltSpeed);
            currentZTilt = newRotation.z;
        }
        else
        {
            newRotation.z = Mathf.Lerp(currentZTilt, 0, tiltSpeed);
            currentZTilt = newRotation.z;
        }
        currentZTilt = Mathf.Clamp(currentZTilt, -maxTilt, maxTilt);

        if (direction.y != 0)
        {
            newRotation.x = Mathf.Lerp(currentXTilt, maxTilt * direction.y, tiltSpeed);
            currentXTilt = newRotation.x;
        }
        else
        {
            newRotation.x = Mathf.Lerp(currentXTilt, 0, tiltSpeed);
            currentXTilt = newRotation.x;
        }
        currentXTilt = Mathf.Clamp(currentXTilt, -maxTilt, maxTilt);

        transform.Rotate(newRotation);
    }

    private void Died()
    {
        if (alive) PopulationManager.numDead++;
        alive = false;

        GetComponentInChildren<TrailRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            rend.enabled = false;
    }

    public GameObject Init(Transform pTransform)
    {
        //initialize DNA

        /* List of possible events that are sensed:
         * 0: enemy stopped //odd  ***0
         * 1: enemy moving  //even ***1
         * 00: player looking at enemy **0*
         * 10: player not looking at enemy  **1*
         * 000: player stopped  *0**
         * 100: player moving   *1**
         * 0000: player within a specified distance 0***
         * 1000: player outside a specified distance 1***
        */

        /* List of possible events that can be performed
         * 0 -> shoot at player;
         * 1 -> shoot in front of player;
         * 2 -> shoot behind player;
         * 3 -> move towards player; 
         * 4 -> stop moving;
         * 5 -> move away from player; 
         * 6 -> strafe left around player; 
         * 7 -> strafe right around player;
        */

        int[] prefValues = new int[DNALength - P_CLOSE];
        for (int i = 0; i < prefValues.Length; i++)
            prefValues[i] = maxDistance;

        int[] values1 = new int[DNALength];
        int[] values2 = new int[DNALength];
        int[] values3 = new int[DNALength];
        for (int i = 0; i < values1.Length; i++)
        {
            values1[i] = possibleEvents1;
            values2[i] = possibleEvents2;
            values3[i] = possibleEvents3;
        }

        preferenceDNA = new DNA(DNALength - P_CLOSE, prefValues);
        shootingDNA = new DNA(DNALength, values1);
        movingDNA = new DNA(DNALength, values2);
        strafingDNA = new DNA(DNALength, values3);
        rb = GetComponent<Rigidbody>();
        timeAlive = 0;
        alive = true;
        playerTransform = pTransform;
        requiredDeltaDistance = distanceCheckIn / Time.fixedDeltaTime * 0.0000001f;
        initialized = true;
        GetComponent<HP>().justDied = new HP.JustDied(Died);
        enemyHeight = transform.position.y;

        return gameObject;
    }

    public Brain InitBrain(Transform pTransform) { Init(pTransform); return this; }
}
