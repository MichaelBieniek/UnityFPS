using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : Photon.MonoBehaviour {

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Animator anim;

    float lastUpdateTime;

	// Use this for initialization
	void Start () {
        MonoBehaviour[] children = GetComponentsInChildren<MonoBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
        if(photonView.isMine)
        {
            // do nothing, local scripts moving us
        } else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            // this is our player sending info about position to network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(anim.GetBool("Aiming"));
            stream.SendNext(anim.GetBool("Attack"));
            stream.SendNext(anim.GetFloat("Speed"));
        }
        else
        {
            // this is someone else sending info about their position
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            anim.SetBool("Aiming", (bool)stream.ReceiveNext());
            anim.SetBool("Attack", (bool)stream.ReceiveNext());
            anim.SetFloat("Speed", (float)stream.ReceiveNext());

        }
    }
}
