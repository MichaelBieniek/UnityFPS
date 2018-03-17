using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    SpawnObject[] spawnSpots;

    [SerializeField]
    GameObject serverCamera;

	// Use this for initialization
	void Start () {
        Connect();
        spawnSpots = GameObject.FindObjectsOfType<SpawnObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Connect()
    {
        //PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("v0.1.0");
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
        
    }

    void OnJoinedRoom()
    {
        SpawnLocalPlayer();
    }

    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    void SpawnLocalPlayer()
    {

        if( spawnSpots == null )
        {
            Debug.LogError("No spawn spots");
            return;
        }
        /*
        SpawnObject myPlayerSpawn = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayerInstance = PhotonNetwork.Instantiate("FPSPlayer", myPlayerSpawn.transform.position, myPlayerSpawn.transform.rotation, 0);
        myPlayerInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        myPlayerInstance.GetComponent<CharacterController>().enabled = true;
        //myPlayerInstance.GetComponentInChildren<Camera>().enabled = true;
        myPlayerInstance.GetComponentInChildren<AudioListener>().enabled = true;
        myPlayerInstance.GetComponentInChildren<GunFire>().enabled = true;

        myPlayerInstance.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        //myPlayerInstance.GetComponentInChildren<RigAssScript>().gameObject.SetActive(false);

        //myPlayerInstance.transform.Find("Soldier").gameObject.SetActive(false);

        //GameObject fpcharacter = myPlayerInstance.transform.Find("FirstPersonCharacter").gameObject;
        //fpcharacter.transform.Find("m4a1-fp").gameObject.SetActive(true);


        // disable server camera
        */
        DisableServerCamera();
    }

    void DisableServerCamera()
    {
        serverCamera.SetActive(false);
    }
}
