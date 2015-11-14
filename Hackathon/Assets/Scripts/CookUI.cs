using UnityEngine;
using System.Collections;

public class CookUI : MonoBehaviour {

    public void Win()
    {
        int sugar = GameManager.instance.playerInventory["Sugar"];
        int strawberry = GameManager.instance.playerInventory["Strawberry"];
        Debug.Log("CookUI");
        if (sugar >= 1 && strawberry >= 1)
        {
            GameObject cake = GameObject.Find("RawImage");
            cake.SetActive(true);
        }
        
        // do nothing
    }
}
