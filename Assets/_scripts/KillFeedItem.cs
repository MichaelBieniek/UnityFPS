using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

    [SerializeField]
    Text text;

    public void Setup(string player, string action, string source)
    {
        if( player == source )
        {
            text.text = "<b>" + source + "</b>" + " " + action + " themsleves";
        } else
        {
            text.text = "<b>" + source + "</b>" + " " + action + " " + "<i>" + player + "</i>";
        }
       
    }
}
