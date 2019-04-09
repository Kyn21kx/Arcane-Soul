using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour {

    #region Variables
    public float spacingTime = 0;
    [SerializeField]
    private bool startTime;
    [SerializeField]
    int btnCntrX = 0, btnCntrY = 0;
    //Light combos controlled by different spacing time between the X button
    public bool a, b, c;
    #endregion

    private void FixedUpdate() {
        XboxInput();
        Timing();
        ComboSelector();
    }

    private void XboxInput () {
        if (Input.GetButtonDown("X")) {
            Debug.Log("Light Attack");
            btnCntrX++;
            spacingTime = 0;
            startTime = true;
        }
        if (Input.GetButtonDown("Y")) {
            Debug.Log("Heavy Attack");
            btnCntrY++;
            spacingTime = 0;
            startTime = true;
        }
    }

    private void Timing () {
        if (startTime) {
            spacingTime += Time.fixedDeltaTime;
        }
        if (spacingTime > 2) {
            btnCntrX = 0;
            btnCntrY = 0;
        }
        if (btnCntrX > 6) {
            btnCntrX = 1;
        }
        if (btnCntrY > 6) {
            btnCntrY = 1;
        }
    }

    private void ComboSelector () {
        if ((spacingTime > 0.5 && spacingTime < 1.5) && btnCntrX >= 2) {
            a = true;
        }
    }

}
