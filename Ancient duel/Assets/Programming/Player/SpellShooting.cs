using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShooting : MonoBehaviour {

    #region Variables
    public GameObject spell;
    #endregion

    private void FixedUpdate() {
        _Input();
    }

    private void _Input() {
        if (Input.GetButtonDown("RB")) {
            Shoot();
        }
    }

    private void Shoot () {
        //Determine mana cost depending of the spell
        if (GetComponent<ManaManager>().manaAmount >= 10f) {
            Instantiate(spell, transform.position, transform.rotation);
            GetComponent<ManaManager>().Reduce(10f);
        }
    }

}
