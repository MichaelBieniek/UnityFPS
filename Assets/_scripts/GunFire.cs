using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GunFire : NetworkBehaviour
{
    FXManager fxManager;

    private const string PLAYER_TAG = "Player";
    public GameObject casing;
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;
    public Animator animator;
    

    public Camera bulletOriginCamera;


    public float effectiveRange = 100f;
    public float muzzleSpeed = 500f;
    public float baseDmg = 5f;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    GameObject currentPlayer;

    [SerializeField]
    AudioSource gunshot;
    [SerializeField]
    AudioSource click;
    [SerializeField]
    AudioSource reload;
    [SerializeField]
    AudioSource autoShot;

    [SerializeField]
    Animation firstPersonAnimations;

    bool _autoFire = false;
    float _actionCoolDown = 0.5f;


    [SerializeField] float fireRate = 800f;
    float _fireRateNorm = 1/(800f/60f);
    float cooldown = 0f;
    float _actionTimer = 0f;

    string _popup = "";

    GUIStyle _guiStyle = new GUIStyle();
    
    public void OnGUI()
    {
        if (_popup != null && _popup != "")
        {
            Debug.Log(_popup);
            GUILayout.BeginArea(new Rect(Screen.width - 200, Screen.height - 35, 200, 80));
            _guiStyle.fontSize = 32;
            GUILayout.Label(_popup, _guiStyle);
            GUILayout.EndArea();
            
        }
    }

    IEnumerator ShowMessage(string message, float delay)
    {
        Debug.Log("show message");
       _popup = message;
        yield return new WaitForSeconds(delay);
       _popup = null;
    }

    private void Start()
    {
        //singleton
        fxManager = GameObject.FindObjectOfType<FXManager>();
        if( fxManager == null)
        {
            Debug.LogError("Can't find FXManager");
        }
    }

    // Update is called once per frame
    void Update()
    {

        cooldown = cooldown <= 0 ? 0 : cooldown - Time.deltaTime;
        _actionTimer = _actionTimer <= 0 ? 0 : _actionTimer - Time.deltaTime;

        if (GlobalAmmo.CurrentAmmo <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                click.Play();
            }
        }
        else
        {
            /*
            // TBD: autofire 
            if (Input.GetButton("Fire1") && Time.time > nextActionTime)
            {
                nextActionTime += period;

                if (!autoShot.isPlaying)
                {
                    autoShot.Play();
                }
                Fire();

            }
            else
            {
                autoShot.Stop();
            }
            */
            if (_autoFire && Input.GetButton("Fire1"))
            {
                Fire();
            } else
            {
                autoShot.Stop();
            }
            if (!_autoFire && Input.GetButtonDown("Fire1"))
            {
                Fire();
            }
            if( !Input.GetButton("Fire1") )
            {
                animator.ResetTrigger("Attack");
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // change fire rate
            if(_actionTimer <= 0f )
            {
                _actionTimer = _actionCoolDown;
                Debug.Log("change mode");
                _autoFire = !_autoFire;
                StartCoroutine(ShowMessage(_autoFire ? "Full auto" : "Single fire", 3f));
            } else
            {
                Debug.Log(_actionTimer);
            }

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        if (Input.GetMouseButton(1))
        {
            Aim();
            //GetComponent<Animation>().Play("Aim");
        }

    }

    void ResetAmmo()
    {
        GlobalAmmo.CurrentAmmo = 30;
        animator.SetBool("Aiming", true);
    }

    IEnumerator FireAnim(float delay)
    {
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(delay);

        animator.ResetTrigger("Attack");
    }
    
    void DropCasing()
    {
        if( casing != null )
        {
            var droppedCasing = (GameObject)Instantiate(casing, muzzleFlash.transform.position + new Vector3(0, -1f, 0.5f), Random.rotation);
            droppedCasing.GetComponent<Rigidbody>().velocity = new Vector3(1, 0, 0) * 5f;
            Destroy(droppedCasing, 10f);
        }
    }

    void Fire()
    {
        if (cooldown > 0)
        {
            // not ready
            return;
        }
        cooldown = _fireRateNorm;

        // play animation & sfx
        DropCasing();

        gunshot.Play();

        animator.SetBool("Aiming", true);

        if (GlobalAmmo.CurrentAmmo <= 0)
        {
            // do nothing
            return;
        }

        // GetComponent<Animation>().Play("GunShot");
        GlobalAmmo.CurrentAmmo -= 1;
        StartCoroutine(FireAnim(0.2f));
        muzzleFlash.GetComponent<ParticleSystem>().Play();
        if (firstPersonAnimations != null)
        {
            firstPersonAnimations.Play("GunShot");
        }

        // Create the Bullet from the Bullet Prefab
        //var bullet = (GameObject)Instantiate(bulletPrefab, bulletOriginCamera.transform.position + new Vector3(0,0,0.5f), bulletOriginCamera.transform.rotation);

        float width = 0;// Random.Range(-1f, 1f) * 1;
        float height = 0;// Random.Range(-1f, 1f) * 1;

        // Add velocity to the bullet
        //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * muzzleSpeed + bullet.transform.right * width + bullet.transform.up * height;

        Ray ray = new Ray(bulletOriginCamera.transform.position+Vector3.forward*1, bulletOriginCamera.transform.forward);



        /*
        RaycastHit closestHit;
        float distance = 0f;

        foreach (RaycastHit hit in hits)
        {
            if( hit.transform != this.transform && hit.distance < 2* effectiveRange) )
            {
                closestHit = hit;
                distance = hit.distance;
            }
        }
        */
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, effectiveRange * 2))
        {
            //DebugDrawLine(bulletOriginCamera.transform.position + Vector3.forward * 1, hit.point, Color.green, 10f);

            Collider target = hit.collider;
            float distance = hit.distance;

            float effDmg = 0;
            if (distance > 2 * effectiveRange)
            {
                //bullet traveled twice the effective range, 0 dmg
                effDmg = 0;
            }
            else if (distance > effectiveRange)
            {
                effDmg = (1 - ((distance - effectiveRange) / effectiveRange)) * baseDmg;
            }
            else
            {
                effDmg = baseDmg;
            }

            Debug.Log(" hit " + target.name + " for " + effDmg + " dmg");

            InflictDamage(target, effDmg, hit);
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

    void Reload()
    {
        reload.Play();
        if (firstPersonAnimations != null)
        {
            Debug.Log("Playing reload anim");
            firstPersonAnimations.Play("Reload");
        }
        animator.ResetTrigger("Attack");
        animator.SetBool("Aiming", false);        
        Invoke("ResetAmmo", 2.0f);
    }

    void Aim()
    {
        bool current = animator.GetBool("Aiming");
        animator.SetBool("Aiming", !current);
    }
    
    private void InflictDamage(Collider _col, float dmg, RaycastHit hit)
    {
        
        Transform colliderTransform = _col.transform;
        Health h = colliderTransform.GetComponent<Health>();
        while(h == null && colliderTransform.parent )
        {
            colliderTransform = colliderTransform.parent;
            h = colliderTransform.GetComponent<Health>();
        }
        if( h != null )
        {

            PhotonView pv = h.GetComponent<PhotonView>();
            if( pv == null )
            {
                // take no action
                //Debug.LogError("No net ID associated with hit obj");
            } else
            {
                pv.RPC("TakeDamage", PhotonTargets.All, dmg);
            }
        }
        //Networked FX
        fxManager.GunShotFX(bulletOriginCamera.transform.position, hit, colliderTransform.tag == "Player" ? FXManager.MaterialType.Player : FXManager.MaterialType.Other);

    }

    public Transform rightGunBone;
    public Transform leftGunBone;
    public Arsenal[] arsenal;

    void Awake()
    {
        if (arsenal.Length > 0)
            SetArsenal(arsenal[0].name);
    }

    public void SetArsenal(string name)
    {
        foreach (Arsenal hand in arsenal)
        {
            if (hand.name == name)
            {
                if (rightGunBone.childCount > 0)
                    Destroy(rightGunBone.GetChild(0).gameObject);
                if (leftGunBone.childCount > 0)
                    Destroy(leftGunBone.GetChild(0).gameObject);
                if (hand.rightGun != null)
                {
                    GameObject newRightGun = (GameObject)Instantiate(hand.rightGun);
                    newRightGun.transform.parent = rightGunBone;
                    newRightGun.transform.localPosition = Vector3.zero;
                    newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                if (hand.leftGun != null)
                {
                    GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
                    newLeftGun.transform.parent = leftGunBone;
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                animator.runtimeAnimatorController = hand.controller;
                Debug.Log("Added weapon " + name);
                return;
            }
        }
    }

    [System.Serializable]
    public struct Arsenal
    {
        public string name;
        public GameObject rightGun;
        public GameObject leftGun;
        public RuntimeAnimatorController controller;
    }

}
