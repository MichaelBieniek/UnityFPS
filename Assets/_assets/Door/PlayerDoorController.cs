using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorController : MonoBehaviour {

    private bool doorInProximity = false;
    float cooldown = 0f;
    PhotonView pvDoor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if( cooldown > 0 )
        {
            cooldown -= Time.deltaTime;
            return;
        }
        if ( Input.GetKeyDown(KeyCode.F) && doorInProximity && pvDoor != null )
        {
            // toggle door action and reset cooldown
            pvDoor.RPC("ToggleDoor", PhotonTargets.All);
            cooldown = 1f;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        Tags tags = other.GetComponent<Tags>();
        if (tags != null && tags.HasTag("Door"))
        {
            doorInProximity = true;
            pvDoor = other.GetComponent<PhotonView>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Tags tags = other.GetComponent<Tags>();
        if (tags != null && tags.HasTag("Door"))
        {
            doorInProximity = false;
            pvDoor = null;
        }
    }
}
