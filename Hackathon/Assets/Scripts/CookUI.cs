using UnityEngine;
using System.Collections;

public class CookUI : MonoBehaviour {

    public void Win()
    {
        int sugar = GameManager.instance.playerInventory["Sugar"];
        int strawberry = GameManager.instance.playerInventory["Strawberry"];
        if (sugar >= 2 && strawberry >= 2)
        {
            GameObject cake = GameObject.Find("RawImage");
            cake.SetActive(true);
        }
        
        // do nothing
    }
}
