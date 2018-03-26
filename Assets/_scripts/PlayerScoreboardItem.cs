using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour {

    [SerializeField]
    Text playerNameText;

    [SerializeField]
    Text killsText;

    [SerializeField]
    Text deathsText;

    public void Setup(string playerName, string kills, string deaths)
    {
        playerNameText.text = playerName;
        killsText.text = "Kills: " + kills;
        deathsText.text = "Deaths: " + deaths;
    }
}
