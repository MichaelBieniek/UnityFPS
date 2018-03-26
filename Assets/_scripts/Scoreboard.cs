using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItemPrefab;

    [SerializeField]
    Transform playerScoreboardList;

    NetworkManager _manager;

    // Use this for initialization
    private void OnEnable()
    {
        // loop through array of players
        _manager = GameObject.FindObjectOfType<NetworkManager>();
        PhotonPlayer[] players = _manager.GetAllPlayers();
        foreach (PhotonPlayer item in players)
        {
            GameObject go = Instantiate(playerScoreboardItemPrefab, playerScoreboardList);
            go.GetComponent<PlayerScoreboardItem>().Setup("Player " + item.ID, (string)item.CustomProperties["kills"], (string)item.CustomProperties["deaths"]);
        }
    }

    private void OnDisable()
    {
        // clean up our list
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
