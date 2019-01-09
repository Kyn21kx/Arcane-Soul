using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAbility : MonoBehaviour {

    #region Variables
    public bool readyToCast = true;
    public float CD;
    #endregion

    public IEnumerator CoolDown () {
        yield return new WaitForSeconds(5f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>().HeavyElectric1.SetActive(false);
        readyToCast = true;
    }

}
