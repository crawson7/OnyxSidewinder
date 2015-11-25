using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OnyxUIController : MonoBehaviour {

    public Text Jumps;
    public Text Distance;
	// Use this for initialization
	public bool Initialize()
    {

        return true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setDistanceText(string s)
    {
        Distance.text = s;
    }

    public void setJumpsText(string s)
    {
        Jumps.text = s;
    }
}
