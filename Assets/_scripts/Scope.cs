using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scope : MonoBehaviour {

    [SerializeField]
    Camera mainCamera;
    Animator _animator;
    bool isScope = false;
    [SerializeField]    Camera weaponCamera;
    [SerializeField]    GameObject scope1;
    [SerializeField]    GameObject scope2;
    [SerializeField]    GameObject crosshair;

    // Use this for initialization
    void Start () {
        //_weaponsCamera = GetComponent<Camera>();
        //_weaponsCamera.enabled = true;
        _animator = gameObject.GetComponent<Animator>();
        scope1.SetActive(false);
        scope2.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire2"))
        {
            if( !isScope )
            {
                StartCoroutine(OnScope());
                _animator.SetBool("Scoped", true);
            }

            _animator.SetBool("Aim", true);
        }
        else
        {
            _animator.SetBool("Scoped", false);
            OnUnScope();
            //unzoom
        }
    }


    void OnUnScope()
    {
        crosshair.SetActive(true);
        scope1.SetActive(false);
        scope2.SetActive(false);
        isScope = false;
        weaponCamera.transform.localPosition = new Vector3(0, 0, 0.0f);
        weaponCamera.enabled = true;
        mainCamera.fieldOfView = 60f;
    }

    IEnumerator OnScope()
    {
        yield return new WaitForSeconds(0.1f);
        crosshair.SetActive(false);
        scope1.SetActive(true);
        scope2.SetActive(true);
        weaponCamera.enabled = false;
        mainCamera.fieldOfView = 15f;
        weaponCamera.transform.localPosition = new Vector3(0, 0, 0.05f);
        isScope = true;
    }
}
