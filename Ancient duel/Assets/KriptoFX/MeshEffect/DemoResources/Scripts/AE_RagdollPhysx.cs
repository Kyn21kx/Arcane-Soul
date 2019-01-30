using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AE_RagdollPhysx : MonoBehaviour
{
    public Animator Animator;

    void Start()
    {
        var bodies = GetComponentsInChildren<Rigidbody>();
            foreach (var body in bodies)
            {
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.mass = 1;
                var temp = body.gameObject.AddComponent<AE_RagdollCollisionEnter>();
                temp.AE_RagdollPhysx = this;
            }
        
        SetKinematic(true);
    }

    public void OnCollisionEnterChild()
    {
        var bodies = GetComponentsInChildren<Rigidbody>();
        foreach (var body in bodies)
        {
            Destroy(body.GetComponent<AE_RagdollCollisionEnter>());
        }
        Death();
    }

    void Death()
    {
        SetKinematic(false);
        Animator.enabled = false;
        Destroy(gameObject, 5);
    }

    void SetKinematic(bool newValue)
    {
        var bodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in bodies)
        {
            rb.isKinematic = newValue;
        }
    }
}
