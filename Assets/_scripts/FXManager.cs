using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

    [SerializeField]
    GameObject bloodSprayPrefab;
    [SerializeField]
    GameObject biWood;
    [SerializeField]
    GameObject biMetal;
    [SerializeField]
    GameObject biConcrete;
    [SerializeField]
    GameObject biOther;
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    AudioSource pickup;

    // grenades
    [SerializeField]
    GameObject explosionPrefab;

    public enum MaterialType {
        Ground = 1,
        Wood = 2,
        Player = 3,
        Other = 4
    };

    public void PlayLocalPickup()
    {
        pickup.Play();
    }

    [PunRPC]
    public void Explosion(Transform source)
    {
        Instantiate(explosionPrefab, source.position, Quaternion.identity);
    }

	[PunRPC] 
    public void GunShotFX(Transform source, RaycastHit hit, MaterialType material)
    {
        //GameObject line = Instantiate(linePrefab, source.position, source.rotation);
        Debug.Log(hit.collider.material);

        if( material == MaterialType.Player )
        {
            GameObject splatter = Instantiate(bloodSprayPrefab, hit.point, Quaternion.identity);
        }
        else
        {
            GameObject bulletImpact = Instantiate(ReturnBulletImpactPrefab(hit.collider.material), hit.point, Quaternion.Euler(hit.transform.forward));
            Destroy(bulletImpact, 15f);
        }
        //GameObject explosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
        //Destroy(explosion, 1);

    }

    GameObject ReturnBulletImpactPrefab(PhysicMaterial material)
    {
        Debug.Log(material.name.ToString());
        if( material.name.ToString() == "Metal (Instance)")
        {
            return biMetal;
        } else if (material.name == "Wood (Instance)")
        {
            return biWood;
        } else
        {
            return biOther;
        }
    }
}
