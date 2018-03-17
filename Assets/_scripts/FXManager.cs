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

    public enum MaterialType {
        Ground = 1,
        Wood = 2,
        Player = 3,
        Other = 4
    };

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
            GameObject bulletHoleInstance = Instantiate(ReturnBulletImpactPrefab(hit.collider.material), hit.point, Quaternion.FromToRotation(hit.point, Vector3.back));
            Destroy(bulletHoleInstance, 15f);
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
