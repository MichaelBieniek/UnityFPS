using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

    [SerializeField]
    float startingAmmo = 30f;
    float _ammo;

    // Use this for initialization
    void Start()
    {
        _ammo = startingAmmo;
    }

    [PunRPC]
    public void PickupAmmo(float amt)
    {
        Debug.Log("Picked up amm");
        _ammo -= amt;
    }

    //TODO: PhotonNetwork.InstantiateSceneObject - masterplayer can inst objects that stay in scene

    // Update is called once per frame
    void Update()
    {

    }
}
