using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSolider : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Animator animator = GetComponent<Animator>();
        animator.SetBool("Aiming", true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
