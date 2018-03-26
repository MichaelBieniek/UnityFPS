using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField]
    GameObject scoreboard;

	// Use this for initialization
	void Start () {
        scoreboard.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Tab) )
        {
            scoreboard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
	}
}
