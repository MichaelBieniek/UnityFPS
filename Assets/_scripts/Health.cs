﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour {

    [SerializeField]
    float maxHealth = 100f;
    float _currentHealth;

    Animator _animator;
    PhotonView _pv;

    public Text uiText;

    // Use this for initialization
    void Start () {
        _currentHealth = maxHealth;
        if( uiText != null )
        {
            uiText.text = "" + _currentHealth;
        }

        Transform soldier = gameObject.transform.Find("Solider");
        if( soldier != null )
        {
            _animator = soldier.GetComponent<Animator>();
        }

        _pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void TakeDamage(float amt, PhotonPlayer source)
    {
        Debug.Log(gameObject.transform.name + " is taking " + amt + " dmg");
        _currentHealth -= amt;
        if(uiText != null)
        {
            uiText.text = "" + _currentHealth;
        }
        if ( _currentHealth <= 0 )
        {
            Die(source);
        }
    }

    //TODO: PhotonNetwork.InstantiateSceneObject - masterplayer can inst objects that stay in scene

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //suicide
            Die(PhotonPlayer.Find(_pv.viewID));
        }
    }

    void UpdateScore(PhotonPlayer player, int killsInc = 0, int deathInc = 0)
    {
        ExitGames.Client.Photon.Hashtable ht = player.CustomProperties;
        if( ht == null )
        {
            ht = new ExitGames.Client.Photon.Hashtable();
        }
        ht["kills"] = (int)ht["kills"] + killsInc;
        ht["deaths"] = (int)ht["deaths"] + deathInc;

        player.SetCustomProperties(ht);
    }

    void Die(PhotonPlayer source)
    {
        if(_pv.instantiationId ==0 )
        {
            //non-networked object
            Destroy(gameObject);
        } else
        {
            if( _pv.isMine)
            {
                if( gameObject.tag == "Player" )
                {
                    PhotonPlayer target = _pv.owner;
                    NetworkManager _manager = GameObject.FindObjectOfType<NetworkManager>();
                    PhotonPlayer[] players = _manager.GetAllPlayers();
                    // give the target a death
                    //UpdateScore(target, 0, 1);
                    // give the origin player a kill
                    //UpdateScore(source, 1, 0);
                    _manager.LogKill(source, target);

                    PlayDeathAnimation();
                    NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
                    nm.serverCamera.SetActive(true);
                    nm.respawnTimer = 3f;
                    Invoke("DestroyObject", 3f);
                    //GUILayout.Label();
                } else
                {
                    DestroyObject();
                }

            } else
            {
                DestroyObject();
            }
            
        }
        // destroy object?
        // play die animation
    }


    void PlayDeathAnimation()
    {
        if( _animator != null )
        {
            Debug.Log("Death anim");
            _animator.SetTrigger("Death");
        }
    }

    void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
        
}
