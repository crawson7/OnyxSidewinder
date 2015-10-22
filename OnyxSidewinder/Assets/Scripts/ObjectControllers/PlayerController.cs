using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Rigidbody2D Body;
    public GameObject SpriteObj;

    public Vector3 Forward { get { return SpriteObj.transform.up; } }
    public float Radius { get { return Mathf.Abs(SpriteObj.transform.localPosition.x); } }

    public bool Initialize()
    {
        return true;
    }
	
	// Update is called once per frame
	void Update () {
	}


}
