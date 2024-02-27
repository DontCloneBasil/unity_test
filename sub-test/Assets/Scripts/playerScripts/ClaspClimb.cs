using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaspClimb : MonoBehaviour
{
    [Header("references")]
    public Transform orientation;
    public Movement pm;
    public Rigidbody rb;
    public LayerMask whatIsGround;

    [Header("climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;

    private bool climbing;

    [Header("clasping")]
    public float claspTimerSlowDown;
    public float claspSpeed;
    private bool clasping;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    [Header("keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode grab = KeyCode.Mouse1;

    // Update is called once per frame
    void Update()
    {
        Wallcheck();
        StateMachine();

        if (climbing || clasping) ClimbingMovement();
    }
    private void StateMachine()
    {
        // state 1 - climbing
        if(wallFront && Input.GetKey(KeyCode.W) && wallLookAngle <= maxWallLookAngle)
        {
            if(!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        // state 2 - clasping
        else if (Input.GetKey(grab))
        {
            if (climbTimer > 0 && wallFront)
            {
                StartClasp();
            }
            if (clasping)
            {
                if (climbTimer > 0) climbTimer -= (Time.deltaTime / claspTimerSlowDown);
                if (climbTimer <= 0) StopClasping();
            }
        }
        // state 3 - none
        else
        {
            if(climbing) StopClimbing();
            if(clasping) StopClasping();
        }
    }
        private void Wallcheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsGround);
        wallLookAngle = Vector3.Angle(orientation.forward, - frontWallHit.normal);

        
        if (pm.grounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StartClimbing()
    {
        if (clasping)
        {
            StopClasping();
        }
        climbing = true;
        pm.climbing = true;
    }
    private void ClimbingMovement()
    {
        if (climbing)
        {
            rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        }
        else if (clasping)
        {
            rb.velocity = new Vector3(rb.velocity.x, claspSpeed, rb.velocity.z);  
        }
    }
    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
    }
    private void StartClasp()
    {
        if (climbing)
        {
            StopClimbing();
        }
        clasping = true;
        pm.clasping = true;
    }
    private void StopClasping()
    {
        clasping = false;
        pm.clasping = false;
    }
}