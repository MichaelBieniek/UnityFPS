using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour {

    bool _doorIsOpen = false;
    float _angleAxis = 90;
    bool _doorOpenInProgress = false;
    [SerializeField] float speed = 50f;
    float _step;

    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource doorClose;

    // Use this for initialization
    void Start () {
		_step = speed * Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
        if( transform.rotation != Quaternion.AngleAxis(_angleAxis, Vector3.up))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(_angleAxis, Vector3.up), _step);
            _doorOpenInProgress = true;
        }
        else
        {
            _doorOpenInProgress = false;
        } 
           
    }

    [PunRPC]
    public void ToggleDoor()
    {
        if (_doorOpenInProgress) return;

        if(_doorIsOpen)
        {
            doorClose.Play();
            _angleAxis = 0;
            _doorIsOpen = false;

        } else
        {
            doorOpen.Play();
            _angleAxis = 90;
            _doorIsOpen = true;
        }        
    }
}
