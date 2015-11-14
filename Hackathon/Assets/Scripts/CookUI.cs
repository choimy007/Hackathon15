using UnityEngine;
using System.Collections;

public class CookUI : MonoBehaviour {

    public void Win()
    {
        int sugar = GameManager.instance.playerInventory["Sugar"];
        int strawberry = GameManager.instance.playerInventory["Strawberry"];
        if (sugar >= 1 && strawberry >= 1)
        {
        	Debug.Log("sugar is " + sugar);
        	Debug.Log("strawberry is " + strawberry);
            GameObject cake = GameObject.Find("RawImage");
            cake.SetActive(true);
        }
        
        // do nothing
    }
}
