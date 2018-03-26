using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour {

    [SerializeField] string[] tags;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool HasTag(string tag)
    {
        foreach (string tagitem in tags)
        {
            if(tagitem == tag)
            {
                return true;
            }
        }
        return false;
    }
}
