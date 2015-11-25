using UnityEngine;
using System.Collections;

public class DebugPrefabController : MonoBehaviour {

    string _debugType;
    public GameObject DebugObject;

    public Vector3 Center { get { return gameObject.transform.position; } }
    // Use this for initialization
    public bool Initialize(string type)
    {
        _debugType = type;
        //if(type.Equals("DebugCameraTarget"))
        gameObject.transform.position = Vector3.zero;
        //setSprite("CircleCross");
        return true;
    }

    public void setSprite(string s)
    {
        DebugObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + s);
    }

    public void setColor(Color color)
    {
        DebugObject.GetComponent<SpriteRenderer>().color = color;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
