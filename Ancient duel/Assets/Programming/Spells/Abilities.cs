using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    #region Variables
    [SerializeField]
    Spells.Abilities abilityManager;
    public bool readyToCast = true;
    #endregion

    private void Start() {

    }

    private void EvaluateAbility () {
        switch (abilityManager) {
            case Spells.Abilities.Heavy1:
                break;
            case Spells.Abilities.Heavy2:
                break;
            case Spells.Abilities.Heavy3:
                break;
            case Spells.Abilities.Heavy4:
                break;
            default:
                break;
        }
    }

    private void Tornado () {
        
    }

    private void Update() {
        abilityManager = GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>().abilitySelector;
    }
}
