using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour {

    #region General variables
    public float stealthSpeed = 4f;
    public float stealthRun = 5.5f;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public Vector2 input;
    float smoothVel = 5f;
    float turnTime = 0.04f;
    public bool grounded;
    public float jumpForce = 20f;
    float distance;
    #endregion

    #region Animation variables
    Animator anim;
    #endregion
    /* TODO:
     * Decrease the movement speed when in the air
     * When in the air, disable movement if parrying
     */
    private void Start() {
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate() {
        MovePlayer();
        Jump();
        grounded = Grounded();
    }

    private void MovePlayer () {
        //Input management
        //if controller input, then change
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 inputDir = input.normalized;
        if (inputDir != Vector2.zero) {
            float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref smoothVel, turnTime);
            //Change walk speed for speed and set the variable depending on the input
            transform.Translate(transform.forward * walkSpeed * Time.fixedDeltaTime, Space.World);
            anim.SetBool("Walk", true);
        }
        else {
            anim.SetBool("Walk", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetButtonDown("Run"))) {
            anim.SetBool("Run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetButtonUp("Run"))) {
            anim.SetBool("Run", false);
        }
    }

    private void Jump () {
        var rb = GetComponent<Rigidbody>();
        if ((Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space)) && grounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        if (distance >= 5) {
            rb.velocity += Vector3.down;
        }
    }

    private bool Grounded () {
        distance = transform.position.y - GameObject.FindGameObjectWithTag("Terrain").transform.position.y;
        if (distance <= 0.5 && distance >= -1) {
            return true;
        }
        else {
            return false;
        }
    }

}
