using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawninator : MonoBehaviour {

    [SerializeField] float xOffset = 50f; // +/- units
    [SerializeField] float zOffset = 50f; // +/- units
    [SerializeField] GameObject objToSpawn;
    [SerializeField] string objToSpawnNetwork;
    [SerializeField] int numToSpawn = 10;
    [SerializeField] float spawnAfterDelay = 0f;
    [SerializeField] float refreshAfter = 0f;
    [SerializeField] float maxDetectorLength = 100f;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BeginSpawn()
    {
        if (objToSpawn != null)
            StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(spawnAfterDelay);
        do
        {
            for (int i = 0; i < numToSpawn; i++)
            {
                SpawnInWorld(gameObject.transform.position, xOffset, zOffset);
            }
            yield return new WaitForSeconds(refreshAfter);
        } while (refreshAfter > 0f);
        
    }

    void SpawnInWorld(Vector3 zero, float xOffset, float zOffset)
    {
        float xAdd = Random.Range(-1 * xOffset, xOffset);
        float zAdd = Random.Range(-1 * zOffset, zOffset);

        Vector3 elevated = zero + new Vector3(xAdd, 0, zAdd);

        Ray ray = new Ray(elevated, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDetectorLength))
        {
            PhotonNetwork.Instantiate("AmmoPickup", hit.point, Quaternion.identity, 0);
        }
    }
}
