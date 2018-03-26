using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour {

    [SerializeField]
    float explodeRadius = 5f;

    [SerializeField]
    float baseDmg = 150f;

    FXManager _fx;

    // Use this for initialization
    void Start () {
        Invoke("Explode", 3.5f);
        _fx = GameObject.FindObjectOfType<FXManager>();
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * 25f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Explode()
    {
        _fx.Explosion(gameObject.transform);
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, explodeRadius, transform.forward, explodeRadius);
        foreach (RaycastHit hit in hits)
        {
            // test if behind cover
            RaycastHit hitLos;
            var exposed = false;
            if (Physics.Raycast(gameObject.transform.position, (hit.transform.position - gameObject.transform.position), out hitLos, explodeRadius))
            {
                exposed = (hitLos.collider == hit.collider);
            }

            float dmgInflicited = ((explodeRadius - hit.distance)/explodeRadius)*baseDmg;
            InflictDamage(hit.collider, dmgInflicited);
        }
        Invoke("DestroySelf", 1f);
    }

    private void InflictDamage(Collider _col, float dmg)
    {

        Transform colliderTransform = _col.transform;
        Health h = colliderTransform.GetComponent<Health>();
        while (h == null && colliderTransform.parent)
        {
            colliderTransform = colliderTransform.parent;
            h = colliderTransform.GetComponent<Health>();
        }
        if (h != null)
        {

            PhotonView pv = h.GetComponent<PhotonView>();
            if (pv == null)
            {
                // take no action
                //Debug.LogError("No net ID associated with hit obj");
            }
            else
            {
                pv.RPC("TakeDamage", PhotonTargets.All, dmg, pv.owner);
            }
        }
       
    }

    void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
