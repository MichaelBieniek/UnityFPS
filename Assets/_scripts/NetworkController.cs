using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : Photon.MonoBehaviour {

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    [SerializeField]
    Animator anim;

    float lastUpdateTime;
    string playerName;

    FXManager _fxManager;

	// Use this for initialization
	void Start () {
        MonoBehaviour[] children = GetComponentsInChildren<MonoBehaviour>();

        //name our player
        this.playerName = "player" + photonView.ownerId;

        _fxManager = GameObject.FindObjectOfType<FXManager>();
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
            stream.SendNext(anim.GetBool("Death"));
            stream.SendNext(anim.GetBool("Aiming"));
            stream.SendNext(anim.GetBool("Attack"));
            stream.SendNext(anim.GetFloat("Speed"));
            stream.SendNext(anim.GetBool("Jump"));
        }
        else
        {
            // this is someone else sending info about their position
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            anim.SetBool("Death", (bool)stream.ReceiveNext());
            anim.SetBool("Aiming", (bool)stream.ReceiveNext());
            anim.SetBool("Attack", (bool)stream.ReceiveNext());
            anim.SetFloat("Speed", (float)stream.ReceiveNext());
            anim.SetBool("Jump", (bool)stream.ReceiveNext());

        }
    }

    [PunRPC]
    public void LogEvent(string source, string action, string target)
    {
        if( action == "killed" )
        {
            KillFeed killfeed = FindObjectOfType<KillFeed>();
            killfeed.OnKill(source, target);
        } else
        {
            Debug.LogError("not implemented");
        }
    }

    [PunRPC]
    public void GunShotFX(Vector3 position)
    {
        _fxManager.GunShotFX(position);
    }

    [PunRPC]
    public void DeathFX(Vector3 position)
    {
        _fxManager.DeathFX(position);
    }
}
