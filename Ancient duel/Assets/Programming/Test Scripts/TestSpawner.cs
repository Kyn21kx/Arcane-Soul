using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour {
    //Test script for the parry mechanic

    public GameObject spell;
    [SerializeField]
    float time = 0f;

	void FixedUpdate () {
        time += Time.fixedDeltaTime;
        if (time >= 2f) {
            Instantiate(spell, transform.position, transform.rotation);
            time = 0f;
        }
	}
}
