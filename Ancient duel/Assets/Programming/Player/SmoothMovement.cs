using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour {

    #region General variables
    public float stealthSpeed = 4f;
    public float stealthRun = 5.5f;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    float speed;
    Vector2 input;
    float smoothVel = 2f;
    float turnTime = 0.2f;
    #endregion

    #region Animation variables
    float xAnim, yAnim;
    Animator anim;
    #endregion

    private void Start() {
        speed = walkSpeed;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        MovePlayer();
        Speed();
        Animate();
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
            transform.Translate(transform.forward * speed * Time.fixedDeltaTime, Space.World);
        }
    }

    void Speed () {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            speed = runSpeed;
            anim.SetBool("Running", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            speed = walkSpeed;
            anim.SetBool("Running", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            speed = stealthSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            speed = walkSpeed;
        }
    }

    private void Animate () {
        anim.SetFloat("X", xAnim);
        anim.SetFloat("Y", yAnim);
    }

}
