// Checks if gameManager has been instantiated.  If not, it instantiates one from from our prefabs

using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
	public GameObject gameManager;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}
	}
}
