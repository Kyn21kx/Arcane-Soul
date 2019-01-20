using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pulse : MonoBehaviour {

    #region Variables
    public PulseAffected[] affectedObjects;
    #endregion

    private void FixedUpdate() {
        IdentifyAffectedObjects();
    }

    private void IdentifyAffectedObjects () {
        affectedObjects = FindObjectsOfType<PulseAffected>();
    }

    public void Pulsate () {
        foreach (var obj in affectedObjects) {
            obj.gameObject.GetComponent<NavMeshAgent>().GetComponent<Rigidbody>().AddForce(12, 12, 12);
        }
    }

}
