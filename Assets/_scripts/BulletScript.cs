using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : Photon.MonoBehaviour
{

    [SerializeField] GameObject splatterPrefab;
    [SerializeField] float muzzleSpeed = 300f;
    [SerializeField] float effRange = 500f;
    [SerializeField] float baseDmg = 20f;
    [SerializeField] float headshotMultiplier = 4f;

    PhotonPlayer _sourcePlayer;
    PhotonView _fxView;
    PhotonView _bulletView;
    object[] _instData;
    Vector3 _startPos;
    Quaternion _startRot;
    Color _color;


    // Use this for initialization
    void Start () {
        Invoke("DestroySelf", 3f);
        _startPos = gameObject.transform.position;
        _startRot = gameObject.transform.rotation;
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * muzzleSpeed;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _fxView = GameObject.FindObjectOfType<FXManager>().gameObject.GetPhotonView();
        _bulletView = gameObject.GetPhotonView();
        _instData = _bulletView != null ? _bulletView.instantiationData : null;
        _sourcePlayer = info.sender;
        if( _instData != null && _instData.Length > 2)
        {
            _color = new Color((float)_instData[0], (float)_instData[1], (float)_instData[2]);
        } else
        {
            _color = Color.red;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float distance = Vector3.Distance(_startPos, gameObject.transform.position);
        Quaternion bulletRot = gameObject.transform.rotation;

        GameObject splat = Instantiate(splatterPrefab, collision.contacts[0].point, Quaternion.FromToRotation(Vector3.forward, collision.contacts[0].normal));
        splat.GetComponentInChildren<SpriteRenderer>().material.color = _color;
        Destroy(splat, 10f);
        Collider target = collision.collider;

        float effDmg = 0;
        if (target.name == "HeadCollider")
        {
            Debug.Log("headshot");
            baseDmg = baseDmg * headshotMultiplier;
        }
        if (distance > 2 * effRange)
        {
            //bullet traveled twice the effective range, 0 dmg
            effDmg = 0;
        }
        else if (distance > effRange)
        {
            effDmg = (1 - ((distance - effRange) / effRange)) * baseDmg;
        }
        else
        {
            effDmg = baseDmg;
        }
        InflictDamage(target, effDmg, collision.contacts[0]);

        if( target.tag == "Permeable" || target.tag == "Player" )
        {
            // not done yet, calculate surface penetration @ 25% attenuation (bullet passes through object)
            Vector3 normal = collision.contacts[0].normal;
            SurfacePen(collision.contacts[0].point, _startRot, effDmg);
        } 
        else
        {
            // ricochet chance
            if( Random.Range(0f,1f) > 0.75f )
            {
                baseDmg = effDmg * 0.25f;
                // will ricochet
                return;
            }
        }

        DestroySelf();
    }

    void SurfacePen(Vector3 penPoint, Quaternion penRot, float effDmg)
    {
        
        float attenuatedDmg = effDmg * 0.25f;   // todo: make this a property of object
        float penRage = 5f; // todo: bullet property

        Ray ray = new Ray(penPoint, -penRot.eulerAngles);
        RaycastHit hit;
        if( Physics.Raycast(ray, out hit, penRage) )
        {
            InflictDamage(hit.collider, attenuatedDmg, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
        }
    }

    void DebugDrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }


    void InflictDamage(Collider col, float dmg, ContactPoint contact)
    {
        Vector3 pos = contact.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        InflictDamage(col, dmg, pos, rot);
    }

    void InflictDamage(Collider _col, float dmg, Vector3 pos, Quaternion rot)
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
                pv.RPC("TakeDamage", PhotonTargets.All, dmg, _sourcePlayer);
            }
        }
        //Networked FX
        _fxView.RPC("HitFX", PhotonTargets.All, pos, rot, colliderTransform.tag == "Player" ? FXManager.MaterialType.Player : FXManager.MaterialType.Other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Player")
        {
            FXManager fx = GameObject.FindObjectOfType<FXManager>();
            fx.BulletWhizLocal();
        }
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
