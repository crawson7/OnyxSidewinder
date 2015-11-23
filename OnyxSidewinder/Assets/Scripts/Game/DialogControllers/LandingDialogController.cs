using UnityEngine;
using System.Collections;

public class LandingDialogController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play()
    {
        Game.Instance.LoadLevel(4);
    }
}
