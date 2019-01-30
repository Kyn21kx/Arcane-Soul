using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AE_RagdollCollisionEnter : MonoBehaviour {

    public AE_RagdollPhysx AE_RagdollPhysx;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.name.Contains("TransformMotion")) return;
        AE_RagdollPhysx.OnCollisionEnterChild();

    }
}
