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
    public Animator animator;

    [SerializeField]
    GameObject grenadePrefab;

    [SerializeField]
    GameObject bulletOriginPoint;
    public GameObject player;
    PhotonView _pv;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    GameObject currentPlayer;

    [SerializeField]
    AudioSource click;
    [SerializeField]
    AudioSource reload;
    [SerializeField]
    AudioSource autoShot;
    [SerializeField]
    ParticleSystem muzzleFlash;
    

    [SerializeField]
    Animation firstPersonAnimations;

    [SerializeField]
    Ammo ammo;

    bool _autoFire = false;
    bool _isFiring = false;
    float _actionCoolDown = 0.5f;


    [SerializeField] float fireRate = 800f;
    float _fireRateNorm = 1/(800f/60f);
    float cooldown = 0f;
    float _actionTimer = 0f;

    string _popup = "";

    GUIStyle _guiStyle = new GUIStyle();

    Transform firstPersonView;
    private Vector3 zeroAngle = Vector3.zero;

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
        FirstPersonView fpv = GameObject.FindObjectOfType<FirstPersonView>();
        firstPersonView = fpv.transform;

        _pv = player.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // restore recoil
        firstPersonView.localRotation = Quaternion.Slerp(firstPersonView.localRotation, Quaternion.identity, Time.deltaTime*3f);

        cooldown = cooldown <= 0 ? 0 : cooldown - Time.deltaTime;
        _actionTimer = _actionTimer <= 0 ? 0 : _actionTimer - Time.deltaTime;

        if (ammo.ammoMagazine <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                click.Play();
            }
        }
        else
        {
            if (_autoFire && Input.GetButton("Fire1"))
            {
                firstPersonView.Rotate(-1, 0, 0);
                Fire();

            } else
            {
                if( _isFiring  )
                {
                    _isFiring = false;
                    //autoShot.Stop();                    
                }
                
            }
            if (!_autoFire && Input.GetButtonDown("Fire1"))
            {
                _isFiring = true;                
                firstPersonView.Rotate(-1, 0, 0);
                Fire();
                _isFiring = false;
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
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowNade();
        }
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
            var droppedCasing = (GameObject)Instantiate(casing, player.transform.position + new Vector3(0, -1f, 0.5f), Random.rotation);
            droppedCasing.GetComponent<Rigidbody>().velocity = new Vector3(1, 0, 0) * 5f;
            Destroy(droppedCasing, 10f);
        }
    }

    void ThrowNade()
    {
        if(cooldown > 0)
        {
            // not ready
            return;
        }
        cooldown = _fireRateNorm;

        // no nades
        if( ammo.grenades <= 0 )
        {
            return;
        }

        GameObject grenade = PhotonNetwork.Instantiate("Grenade", player.transform.position + new Vector3(0, 1, 1), player.transform.rotation, 0);
        ammo.UseNade();
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

        muzzleFlash.Play();
        _pv.RPC("GunShotFX", PhotonTargets.All, muzzleFlash.transform.position);

        //gunshot.Play();

        animator.SetBool("Aiming", true);

        if (ammo.ammoMagazine <= 0)
        {
            // do nothing
            return;
        }

        // GetComponent<Animation>().Play("GunShot");
        ammo.UseRound();
        StartCoroutine(FireAnim(0.2f));
        
        if (firstPersonAnimations != null)
        {
            firstPersonAnimations.Play("GunShot");
        }
        // Create the Bullet from the Bullet Prefab
        GameObject bullet = (GameObject)PhotonNetwork.Instantiate("Bullet", bulletOriginPoint.transform.position, bulletOriginPoint.transform.rotation, 0, _pv.instantiationData);

        // Add velocity to the bullet
        //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * muzzleSpeed + bullet.transform.right * width + bullet.transform.up * height;

        //Ray ray = bulletOriginCamera.ScreenPointToRay(Input.mousePosition);


        /*
        // raycast logic commented
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, effectiveRange * 2))
        {
            //DebugDrawLine(Input.mousePosition, hit.point, Color.green, 10f);

            Collider target = hit.collider;
            
            float distance = hit.distance;

            float effDmg = 0;
            if (target.name == "HeadCollider")
            {
                Debug.Log("headshot");
                baseDmg = baseDmg * 4;
            }
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
        */

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
        ammo.Reload();
    }

    void Aim()
    {
        bool current = animator.GetBool("Aiming");
        animator.SetBool("Aiming", !current);
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
                //Debug.Log("Added weapon " + name);
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
