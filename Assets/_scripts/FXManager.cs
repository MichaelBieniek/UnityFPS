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
    GameObject muzzleFlash;
    [SerializeField]
    AudioClip gunshot;
    [SerializeField]
    AudioSource bulletWhiz;

    [SerializeField]
    AudioSource pickup;

    [SerializeField] AudioSource hurt;
    [SerializeField] AudioClip hurt1;
    [SerializeField] AudioClip hurt2;
    [SerializeField] AudioClip hurt3;
    [SerializeField] AudioClip hurt4;

    [SerializeField] AudioClip death;

    // grenades
    [SerializeField]
    GameObject explosionPrefab;

    public enum MaterialType {
        Metal = 0,
        Ground = 1,
        Wood = 2,
        Player = 3,
        Stone = 4,
        Other = 9,
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

    public void DeathFX(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(death, position);
    }

    public void GunShotFX(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(gunshot, position);

        // muzzle flash
        GameObject go = Instantiate(muzzleFlash, position, Quaternion.identity);
        Destroy(go, 0.3f);
    }

    [PunRPC]
    public void HitFX(Vector3 hitPoint, Quaternion hitForward, MaterialType material)
    {
        
        if( material == MaterialType.Player )
        {
            GameObject splatter = Instantiate(bloodSprayPrefab, hitPoint, Quaternion.identity);
            int clip = (int)Random.Range(0, 4);
            switch( clip )
            {
                case 0:
                    AudioSource.PlayClipAtPoint(hurt1, hitPoint);
                    break;
                case 1:
                    AudioSource.PlayClipAtPoint(hurt2, hitPoint);
                    break;
                case 2:
                    AudioSource.PlayClipAtPoint(hurt3, hitPoint);
                    break;
                default:
                    AudioSource.PlayClipAtPoint(hurt4, hitPoint);
                    break;
            }            
        }
        else
        {
            GameObject bulletImpact = Instantiate(ReturnBulletImpactPrefab(material), hitPoint, hitForward);
            Destroy(bulletImpact, 15f);
        }
        //GameObject explosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
        //Destroy(explosion, 1);

    }

    GameObject ReturnBulletImpactPrefab(MaterialType material)
    {
        if( material == MaterialType.Metal)
        {
            return biMetal;
        }
        else if (material == MaterialType.Wood)
        {
            return biWood;
        }
        else if (material == MaterialType.Stone)
        {
            return biConcrete;
        }
        else
        {
            return biOther;
        }
    }

    public void BulletWhizLocal()
    {
        Debug.Log("play whiz");
        bulletWhiz.Play();
    }
}
