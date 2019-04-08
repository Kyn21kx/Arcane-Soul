using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour {

    //Combos by enum
    //Light combos controlled by different spacing time between the X button
    public enum LightCombos {A, B, C};
    //Heavy combos controlled by different spacing time between the Y button
    public enum HeavyCombos {_A, _B, _C };
    //Complex powerful combos controlled by different spacintg time between both X and Y buttons
    public enum MixedCombos { __A, __B, __C };

    #region Variables
    public float spacingTime = 0;
    [SerializeField]
    private bool startTime;
    [SerializeField]
    int btnCntrX = 0, btnCntrY = 0;
    LightCombos lightCombos;
    HeavyCombos heavyCombos;
    MixedCombos mixedCombos;
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
        if (btnCntrX > 6 || btnCntrY > 6) {
            btnCntrX = 1;
            btnCntrY = 1;
        }
    }

    private void ComboSelector () {
        if (btnCntrX >= 1 && spacingTime < 0.5 && (lightCombos != LightCombos.B || lightCombos != LightCombos.C)) {
            lightCombos = LightCombos.A;
        }
        if (btnCntrX >= 2 && spacingTime > 0.5 && spacingTime < 1) {
            lightCombos = LightCombos.B;
        }
        
    }

}
