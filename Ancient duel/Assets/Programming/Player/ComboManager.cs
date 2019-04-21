using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour {

    /*TODO:
     * Limit the maximum times an input can be pressed
     * Add all heavy and light combos
     * Add mixed combos
     * Fix animation with input
    */
    //Enums for the combos
    public enum Combos_Master {Default, A, B, C, Airbone, Dash};

    #region Variables
    public bool lockOn;
    [SerializeField]
    float spacingTime = 0f;
    [SerializeField]
    private bool startTime;
    //***Controls time and hits of input for the combo to reset
    [SerializeField]
    float maxTime = 3f;
    [SerializeField]
    int maxInput = 4;
    //***********
    [SerializeField]
    int btnCntrX = 0, btnCntrY = 0;
    [SerializeField]
    bool preserveCombo = false;
    public float previousSpacingTime;
    //Light combos controlled by different spacing time between the X button
    public Combos_Master lightCombo;
    #endregion

    private void FixedUpdate() {
        XboxInput();
        TimeStart();
        Combo_Selector();
    }

    private void XboxInput () {
        if (Input.GetButtonDown("X")) {
            btnCntrX++;
            previousSpacingTime = spacingTime;
            spacingTime = 0;
            startTime = true;
        }
    }

    private void Combo_Selector () {
        if (!preserveCombo) {
            if (previousSpacingTime >= 1 && previousSpacingTime <= 2 && btnCntrX == 2) {
                lightCombo = Combos_Master.A;
                preserveCombo = true;

            }
            else if (previousSpacingTime >= 1 && previousSpacingTime <= 2 && btnCntrX == 3) {
                lightCombo = Combos_Master.B;
                maxInput = 8;
                preserveCombo = true;
            }
            #region Lock On
            else if (Input.GetButtonDown("RB")) {
                lockOn = true;
            }
            else if (Input.GetButtonUp("RB")) {
                lockOn = false;
            }
            #endregion
            else {
                lightCombo = Combos_Master.Default;
                //preserveCombo = true;
                maxInput = 4;
            }
        }
    }

    private void TimeStart () {
        if (startTime) {
            spacingTime += Time.fixedDeltaTime;
            spacingTime = Mathf.Clamp(spacingTime, 0f, maxTime);
        }
        //Clear time once it gets to the maximum time allowed (maxTime)
        #region Clear
        if (spacingTime >= maxTime) {
            spacingTime = 0;
            startTime = false;
            btnCntrX = 0;
            preserveCombo = false;
            btnCntrY = 0;
        }
        if (btnCntrX > maxInput) {
            btnCntrX = 1;
            preserveCombo = false;
            spacingTime = 0;
        }
        if (btnCntrY > maxInput) {
            btnCntrY = 1;
            preserveCombo = false;
            spacingTime = 0;
        }
        #endregion
    }

}
