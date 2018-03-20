using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    SpawnObject[] spawnSpots;

    public GameObject serverCamera;

    public float respawnTimer = 0f;
    // Use this for initialization
	void Start () {
        Connect();
        spawnSpots = GameObject.FindObjectsOfType<SpawnObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if(respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
            if(respawnTimer <= 0 )
            {
                // respawn;
                SpawnLocalPlayer();

            }
        }

    }

    void Connect()
    {
        //PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("v0.1.0");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 100, 50));
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label(PhotonNetwork.networkingPeer.RoundTripTime + "");
        GUILayout.EndArea();

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
        
        SpawnObject myPlayerSpawn = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayerInstance = PhotonNetwork.Instantiate("FPSPlayer", myPlayerSpawn.transform.position, myPlayerSpawn.transform.rotation, 0);


        myPlayerInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        myPlayerInstance.GetComponent<CharacterController>().enabled = true;

        ReactivateChildren(myPlayerInstance.transform.Find("FirstPersonView").gameObject, true);

        Camera[] cameras = myPlayerInstance.GetComponentsInChildren<Camera>();
        foreach (Camera item in cameras)
        {
            item.gameObject.SetActive(false);
            item.gameObject.SetActive(true);
        }

        myPlayerInstance.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        //myPlayerInstance.GetComponentInChildren<RigAssScript>().gameObject.SetActive(false);

        //myPlayerInstance.transform.Find("Soldier").gameObject.SetActive(false);

        //GameObject fpcharacter = myPlayerInstance.transform.Find("FirstPersonCharacter").gameObject;
        //fpcharacter.transform.Find("m4a1-fp").gameObject.SetActive(true);


        // disable server camera
        
        DisableServerCamera();
    }

    void ReactivateChildren(GameObject g, bool a)
    {
        g.SetActive(a);

        foreach (Transform child in g.transform)
        {
            ReactivateChildren(child.gameObject, a);
        }
    }

    void DisableServerCamera()
    {
        serverCamera.SetActive(false);
    }

    public PhotonPlayer[] GetAllPlayers()
    {
        return PhotonNetwork.playerList;
    }

    [PunRPC]
    public void LogKill(PhotonPlayer source, PhotonPlayer target)
    {
        Debug.Log(source.ID + " killed " + target.ID);
    }
}
