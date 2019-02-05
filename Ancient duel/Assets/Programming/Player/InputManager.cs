using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Variables
    public bool xbox, ps4, pc;
    RuntimePlatform manager;
    #endregion

    private void Update() {
        switch (manager) {
            case RuntimePlatform.PS4:
                ps4 = true;
                break;
            case RuntimePlatform.XboxOne:
                xbox = true;
                break;
            default:
                pc = true;
                break;
        }
    }

}
