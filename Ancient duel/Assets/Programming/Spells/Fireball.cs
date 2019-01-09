using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    #region Variables
    public bool readytoCast = true;
    #endregion
    

    public void Casted() {
        readytoCast = false;
        StartCoroutine(ChargeCD());
    }

    IEnumerator ChargeCD () {
        yield return new WaitForSecondsRealtime(0.45f);
        readytoCast = true;
    }

}
