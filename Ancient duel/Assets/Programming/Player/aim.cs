using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aim : MonoBehaviour {

    #region Variables
    [SerializeField]
    Camera mainCam;
    GameObject Player;
    vThirdPersonController thirdPersonController;
    vThirdPersonCamera Properties;
    public GameObject holder;
    public bool aiming;
    public bool locked;
    float fieldOfView = 27f;
    float autoAimSpeed = 5f;
    Canvas crosshair;
    Transform to;
    #endregion
    //Instead of Set active do camera.enabled
    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        thirdPersonController = Player.GetComponent<vThirdPersonController>();
        mainCam = Camera.main;
        Properties = mainCam.GetComponent<vThirdPersonCamera>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Canvas>();
        crosshair.enabled = false;
        locked = false;
    }

    void AimMode() {
        //LockAim();
        //Si se conecta un control cambiar el Input
        //Opción de apuntar en izquierda o derecha
        if (Input.GetKeyUp(KeyCode.Q) && !aiming) {
            mainCam.fieldOfView = fieldOfView;
            Properties.rightOffset = 0.1f;
            Properties.defaultDistance = 7f;
            Properties.height = 3.78f;
            crosshair.enabled = true;
            thirdPersonController.freeRunningSpeed = thirdPersonController.freeRunningSpeed * (0.5f);
            thirdPersonController.freeSprintSpeed = thirdPersonController.freeSprintSpeed * (0.5f);
            aiming = true;
            //StartCoroutine(Stick());
        }
        else if (Input.GetKeyUp(KeyCode.Q) && aiming) {
            mainCam.fieldOfView = 47f;
            mainCam.GetComponent<vThirdPersonCamera>().rightOffset = 0f;
            Properties.defaultDistance = 7.53f;
            Properties.height = 3.73f;
            thirdPersonController.freeRunningSpeed = 7f;
            thirdPersonController.freeSprintSpeed = 8f;
            aiming = false;
            //StartCoroutine(Stick());
            crosshair.enabled = false;

        }
    }

    private void Update() {
        AimMode();
        //FixCrosshairPosition();
    }
    #region Provisional global variables
    Vector2 viewportTarget;
    #endregion
    void LockAim() {
        RaycastHit hit;
        float distance = 60f;
        Vector2 viewportPlayer = mainCam.WorldToViewportPoint(Player.transform.position);
        
        if ((aiming && Physics.Raycast(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward), out hit, distance)) && hit.transform.CompareTag("Enemy")) {

            if (Input.GetKeyUp(KeyCode.R) && !locked) {
                to = hit.transform;
                viewportTarget = mainCam.WorldToViewportPoint(hit.transform.position);
                locked = true;
            }
            else if (Input.GetKeyUp(KeyCode.R) && locked) {
                to = null;
                viewportTarget = new Vector2(0, 0);
                locked = false;
            }
        }
        else {
            Properties.xMouseSensitivity = 3.5f;
            Properties.yMouseSensitivity = 3.5f;
        }
        switch (locked) {

            case true:
                viewportPlayer = Camera.main.WorldToViewportPoint(Player.transform.position);
                Vector2 viewportTarget = Camera.main.WorldToViewportPoint(to.position);
                float viewportDistance = Vector2.Distance(viewportPlayer, viewportTarget);
                Debug.LogFormat(viewportDistance.ToString(), Color.blue);
                Vector3 dir = to.position - Camera.main.transform.position;
                Quaternion rot = Quaternion.LookRotation(dir * 0.5f);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, rot, 50f * Time.fixedDeltaTime);
                if (viewportDistance >= 0.9f) {
                    Properties.defaultDistance += 20f * Time.fixedDeltaTime;
                    Properties.defaultDistance = Mathf.Clamp(Properties.defaultDistance, Properties.defaultDistance, 100f);
                }
                else if (viewportDistance < 0.8) {
                    Properties.defaultDistance -= 20f * Time.fixedDeltaTime;
                }
                
                break;
        }
    }

    private void FixCrosshairPosition () {
        var crossPosition = crosshair.GetComponentInChildren<RectTransform>();
        crossPosition.transform.position = mainCam.WorldToScreenPoint(holder.transform.position);
    }

}
