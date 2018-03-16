using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalAmmo : MonoBehaviour {

   public static int CurrentAmmo = 30;

    private int InternalAmmo;
    [SerializeField]
    GameObject AmmoDisplay;

	// Update is called once per frame
	void Update () {

        InternalAmmo = CurrentAmmo;
        if( InternalAmmo <= 0 )
        {
            InternalAmmo = 0;
        }
        AmmoDisplay.GetComponent<Text>().text = "Ammo: " + InternalAmmo;


    }
}
