using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AE_SimpleDecal : MonoBehaviour
{
    public float Offset = 0.05f;
    private Transform t;
    bool canUpdate;
    
	// Use this for initialization
	void Awake ()
	{
	    t = transform;
    }

    private void OnEnable()
    {
        canUpdate = true;
        GetComponent<MeshRenderer>().enabled = true;
        UpdatePosition();
        InvokeRepeating("UpdatePosition", 0, 0.2f);
    }

    private RaycastHit hit;
    // Update is called once per frame
    void UpdatePosition ()
	{
        if (!canUpdate) return;
      
	    if (Physics.Raycast(t.parent.position - t.forward / 2, t.forward, out hit))
	    {
            var skinnedMesh = hit.transform.root.GetComponentInChildren<SkinnedMeshRenderer>();

            if (skinnedMesh != null)
            {
                GetComponent<MeshRenderer>().enabled = false;
                return;
            }
            transform.position = hit.point - transform.forward * Offset;
            transform.rotation = Quaternion.LookRotation(-hit.normal);
	    }
	}
    

    private void OnDisable()
    {
        canUpdate = false; //can't use cancelInvoke, because it's caused crash
    }
}
