using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.CharacterController;

public class RangedAbility : MonoBehaviour {

    #region Variables
    public bool readyToCast = true;
    public float CD;
    private vThirdPersonCamera mainCamera;
    #endregion

    private void Start() {
        mainCamera = Camera.main.GetComponent<vThirdPersonCamera>();
    }

    public IEnumerator CoolDown () {
        yield return new WaitForSeconds(10f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>().HeavyElectric1.SetActive(false);
        readyToCast = true;
    }

    public void AdjustCamera() {
        
    }

}
