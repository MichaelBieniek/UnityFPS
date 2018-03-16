using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Health : MonoBehaviour {

    [SerializeField]
    float maxHealth = 100f;
    float _currentHealth;

    // Use this for initialization
    void Start () {
        _currentHealth = maxHealth;
	}

    [PunRPC]
    public void TakeDamage(float amt)
    {
        Debug.Log(gameObject.transform.name + " is taking " + amt + " dmg");
        _currentHealth -= amt;
        if( _currentHealth <= 0 )
        {
            Die();
        }
    }

    //TODO: PhotonNetwork.InstantiateSceneObject - masterplayer can inst objects that stay in scene

    // Update is called once per frame
    void Update () {
		
	}

    void Die()
    {
        // destroy object?
        Destroy(gameObject);
    }
}
