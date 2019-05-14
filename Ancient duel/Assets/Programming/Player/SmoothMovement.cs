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
    bool grounded;
    #endregion

    #region Animation variables
    float xAnim, yAnim;
    Animator anim;
    #endregion

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
        xAnim = input.x;
        yAnim = input.y;
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
        if ((Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space)) && grounded) {
                GetComponent<Rigidbody>().velocity = Vector3.up * (10f);
        }
    }

    private bool Grounded () {
        //Find the nearest ground to the player
        //Find the distance in the y axis of that ground
        float distance = transform.position.y - GameObject.FindGameObjectWithTag("Terrain").transform.position.y;
        //Debug.Log(distance);
        if (distance <= 0.5) {
            return true;
        }
        else {
            return false;
        }
    }

}
