using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleNetworkController : Photon.MonoBehaviour
{
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            // do nothing, local scripts moving us
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // this is our player sending info about position to network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // this is someone else sending info about their position
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
