using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxScript : MonoBehaviour {

    void Start()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Ammo box collision");
        if( collider.tag == "Player")
        {
            PhotonView pv = collider.GetComponent<PhotonView>();
            if( pv != null )
            {
                GameObject.FindObjectOfType<FXManager>().PlayLocalPickup();
                pv.RPC("PickupAmmo", PhotonTargets.All, 100, 3);                
                gameObject.GetComponent<PhotonView>().RPC("DestroyAmmoBox", PhotonTargets.All);
            }


        }
    }

    [PunRPC]
    void DestroyAmmoBox()
    {
        Destroy(gameObject);
    }
}
