using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : MonoBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisableRemote;
    [SerializeField]
    Behaviour[] componentsToDisableLocal;
    Camera serverCamera;

    private void Start()
    {

        serverCamera = Camera.main;
        if (serverCamera != null)
        {
            serverCamera.GetComponent<AudioListener>().enabled = false;
            serverCamera.gameObject.SetActive(false);
        }

        Player player = gameObject.GetComponent<Player>();
        /*
        Transform soldier = player.transform.Find("Soldier");
        soldier.gameObject.SetActive(false);

        // disabled local player mesh and animator
        SkinnedMeshRenderer[] arr = soldier.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in arr)
        {
            if (item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
        Transform[] transformArr = soldier.gameObject.GetComponentsInChildren<Transform>();
        foreach (var item in transformArr)
        {
            if (item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }

        }
        Collider[] colliderArr = soldier.gameObject.GetComponentsInChildren<Collider>();
        foreach (var item in colliderArr)
        {
            if (item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }

        Animator soldierAnimator = soldier.GetComponent<Animator>();
        if (soldierAnimator != null)
        {
            soldierAnimator.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        */
        player.Setup();
    }

    /*
	// Use this for initialization
	void Start () {

		if( !isLocalPlayer )
        {
            for( int i = 0; i < componentsToDisableRemote.Length; i++)
            {
                componentsToDisableRemote[i].enabled = false;
            }
            GameObject[] fpvOnly = GameObject.FindGameObjectsWithTag("FirstPersonViewOnly");
            foreach (var item in fpvOnly)
            {
                if(item != null)
                {
                    Destroy(item);
                }
            }

        }
        else
        {

            serverCamera = Camera.main;
            if( serverCamera != null )
            {
                serverCamera.GetComponent<AudioListener>().enabled = false;
                serverCamera.gameObject.SetActive(false);
            }            
        }

        MonoBehaviour[] allComp = gameObject.GetComponents<MonoBehaviour>();
        for (int i = 0; i < allComp.Length; i++)
        {
            Debug.Log(allComp[i].GetType());
        }
        Player player = gameObject.GetComponent<Player>();
        player.Setup();
	}

    
    // Connect
    // Overrides OnStartClient, registers player with GameManager
    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }
    

    // used for activating cameras and input
    public override void OnStartLocalPlayer()
    {
        Debug.Log("Local player start");
        
        Player p = GetComponent<Player>();

        p.CmdSetHealth();

        Transform soldier = p.transform.Find("Soldier");
        soldier.gameObject.SetActive(false);

        // disabled local player mesh and animator
        SkinnedMeshRenderer[] arr = soldier.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in arr)
        {
            if (item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
        Transform[] transformArr = soldier.gameObject.GetComponentsInChildren<Transform>();
        foreach (var item in transformArr)
        {
            if( item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            } else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            
        }
        Collider[] colliderArr = soldier.gameObject.GetComponentsInChildren<Collider>();
        foreach (var item in colliderArr)
        {
            if (item.gameObject.CompareTag("FirstPersonViewOnly"))
            {
                // skip FPV only items
                continue;
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
        
        Animator soldierAnimator = soldier.GetComponent<Animator>();
        if(soldierAnimator != null)
        {
            soldierAnimator.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    // Disconnect
    private void OnDisable()
    {
        if(serverCamera != null)
        {
            serverCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
    */
}
