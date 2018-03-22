using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Ammo : MonoBehaviour {

    [SerializeField]
    int startingAmmo = 30;
    [SerializeField]
    int startingGrenades = 3;
    public int ammoTotal { get; private set; }
    public int ammoMagazine { get; private set; }
    public int grenades { get; private set; }
    public Text uiText;
    public Image gr1;
    public Image gr2;
    public Image gr3;

    // Use this for initialization
    void Start()
    {
        ammoTotal = startingAmmo;
        ammoMagazine = startingAmmo;
        grenades = startingGrenades;
        UpdateUI();
    }

    void UpdateUI()
    {
        //Debug.Log("Updating UI");
        uiText.text = "" + ammoMagazine + "/" + ammoTotal;
        if( ammoTotal == 0 && ammoMagazine == 0)
        {
            uiText.color = Color.red;
        }
        gr1.enabled = false;
        gr2.enabled = false;
        gr3.enabled = false;
        if( grenades > 0 )
        {
            gr1.enabled = true;
        }
        if( grenades > 1 )
        {
            gr2.enabled = true;
        }
        if (grenades>2)
        {
            gr3.enabled = true;
        }
    }

    [PunRPC]
    public void PickupAmmo(int ammo, int grenades)
    {
        Debug.Log("Picked up amm");
        ammoTotal += ammo;
        this.grenades = grenades;
        UpdateUI();
    }

    public void Reload()
    {        
        StartCoroutine(FlashReload());        
    }

    IEnumerator FlashReload()
    {
        uiText.CrossFadeAlpha(0.0f, 0.5f, false);

        yield return new WaitForSeconds(.5f);
        uiText.CrossFadeAlpha(1.0f, 0.5f, false);

        yield return new WaitForSeconds(.5f);
        uiText.CrossFadeAlpha(0.0f, 0.5f, false);

        yield return new WaitForSeconds(.5f);
        uiText.CrossFadeAlpha(1.0f, 0.5f, false);

        int putBack = ammoMagazine;
        int reloadAmt = Math.Min(ammoTotal + ammoMagazine, 30);
        ammoTotal += putBack;
        ammoTotal -= reloadAmt;
        ammoMagazine = reloadAmt;
        UpdateUI();
    }

    public void UseRound()
    {
        ammoMagazine--;
        Debug.Log("Round expended, left: " + ammoMagazine);
        UpdateUI();
    }

    public void UseNade()
    {
        grenades--;
        if (grenades < 0) grenades = 0;
        UpdateUI();
    }

    //TODO: PhotonNetwork.InstantiateSceneObject - masterplayer can inst objects that stay in scene

    // Update is called once per frame
    void Update()
    {
        
    }
}
