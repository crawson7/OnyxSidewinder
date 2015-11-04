using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {
    public GameObject PlanetObject;
    public GameObject GravityObject;
    public bool Active; // Determines if this planet is to be considered in the collisiton tests.
	public bool full = false;
	public int ID;

    public Vector3 Center { get { return gameObject.transform.position; } }
    public float BodyRadius { get; set; }
    public float GravityRadius { get; set; }


    public void Initialize(float body, float gravity)
    {
        Active = true;
        SetBodySize(body);
        SetGravitySize(gravity);
    }

    // Update is called once per frame
    void Update() {
    }

    private void SetBodySize(float body)
    {
        BodyRadius = body;
        PlanetObject.transform.localScale = Vector3.one * body * 2.0f;
    }

    private void SetGravitySize(float gravity)
    {
        GravityRadius = gravity;
        GravityObject.transform.localScale = Vector3.one * gravity * 2.0f;
    }

    public void Reset()
    {
    }
    
    public bool TestCollision()
    {
        Vector3 pos = Game.Instance.Player.Position;
        if (BodyCollision(pos))
        {
            Logger.Log("Body Collide");
            Game.Instance.HandlePlanetCollide(this);
            return true;
        }
        else if (GravityCollision(pos))
        {
            Logger.Log("Gravity Collide");
            Game.Instance.HandleGravityCollide(this);
            return true;
        }
        return false;
    }

    public bool BodyCollision(Vector3 pos)
    {
        float dist = Vector3.Magnitude(pos - Center);
        return (dist<=BodyRadius);
    }

    public bool GravityCollision(Vector3 pos)
    {
        float dist = Vector3.Magnitude(pos - Center);
        Vector3 planetPosRelativeToPlayer = Game.Instance.Player.PlayerObject.transform.InverseTransformPoint(gameObject.transform.position);
        return (dist <= GravityRadius && planetPosRelativeToPlayer.z <= 0);
    }

}
