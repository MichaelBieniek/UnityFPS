using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleController : MonoBehaviour {


    Vector3 force;
    Vector3 relativeTorque;
    [SerializeField]
    float homingSensitivity = 1f;

    [SerializeField]
    ParticleSystem explosion_aerial;

    [SerializeField]
    ParticleSystem explosion_ground;

    public int id;

    GameObject rocketBody;
    
    [SerializeField]
    float inflightThrust = 20f;

    Quaternion origin;

    [SerializeField]
    Transform target;

    bool _isArmed = false;
    bool _guidanceSystemActive = false;
    bool _stage1Spread = false;

    // Use this for initialization
    void Start () {
        Invoke("Stage1Spread", 3f);
        Invoke("GuidanceSystem", Random.Range(7f,7.5f));
        Invoke("AutoDetonate", 35f);
        Invoke("Arm", 2f);
        target = GameObject.FindGameObjectWithTag("Target").transform;
        rocketBody = gameObject;
        if( id % 2 == 0 )
        {
            origin=Quaternion.LookRotation(Vector3.right);
        }
        else
        {
            origin=Quaternion.LookRotation(Vector3.up);
        }
        offset = Vector3.right * Random.Range(-0.3F, 0.3F);
    }
    float frame = 0;

    Vector3 offset;

    // Update is called once per frame
    void Update () {

        // keep adding force
        if( gameObject == null )
        {
            return;
        }

        Transform missle = rocketBody.transform;
        if (_guidanceSystemActive && target != null)
        {
            rocketBody.GetComponent<ConstantForce>().force = Vector3.zero;
            Vector3 targetDelta = target.position - missle.position;
            Quaternion targetDeltaQ = Quaternion.LookRotation(targetDelta);
            float turn = (Time.deltaTime * 100)/1f;
            Quaternion incRot = Quaternion.Slerp(missle.rotation, targetDeltaQ, 2.5f);
            rocketBody.GetComponent<Rigidbody>().MoveRotation(incRot);

        } else
        {
            if(_stage1Spread)
            {
                rocketBody.GetComponent<ConstantForce>().force = Vector3.zero;                



                float turn = (Time.deltaTime * 100) / 1f;
                Quaternion incRot = Quaternion.RotateTowards(missle.rotation, origin, -1f);
                rocketBody.GetComponent<Rigidbody>().MoveRotation(incRot);
            }
            rocketBody.GetComponent<ConstantForce>().relativeForce = (Vector3.forward) * inflightThrust;
        }
    }

    void Arm()
    {
        _isArmed = true;
    }

    void GuidanceSystem()
    {
        _guidanceSystemActive = true;
        _stage1Spread = false;
    }

    void Stage1Spread()
    {
        _stage1Spread = true;
    }

    void Explode(bool inAir)
    {

        if(inAir)
        {
            Instantiate(explosion_aerial, rocketBody.transform.position, Quaternion.identity);
        } else
        {
            Instantiate(explosion_ground, rocketBody.transform.position, Quaternion.identity);
        }
        
        PhotonNetwork.Destroy(gameObject);

        /*
        foreach (Transform child in gameObject.GetComponentInChildren<Transform>())
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(GetComponent<ConstantForce>(), 2f);
        GameObject.Destroy(GetComponent<Rigidbody>(), 2f);
        */
    }

    void AutoDetonate()
    {
        Explode(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode(false);
        Collider[] colliders = Physics.OverlapSphere(collision.transform.position, 10f);
        foreach (var col in colliders)
        {
            InflictDamage(col, 1000f);
        }
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
}
