using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {

    public bool Active; // Determines if this planet is to be considered in the collisiton tests.

    public Vector3 Center { get { return gameObject.transform.position; } }
    public float BodyRadius { get; set; }
    public float GravityRadius { get; set; }


    public void Initialize(float body, float gravity)
    {
        BodyRadius = body;
        GravityRadius = gravity;
        Active = true;
    }
	
	// Update is called once per frame
	void Update () {
    }

    public bool TestCollision()
    {
        Vector3 pos = Game.Instance.Player.Position;
        if (BodyCollision(pos))
        {
            Game.Instance.HandlePlanetCollide(this);
            return true;
        }
        else if (GravityCollision(pos))
        {
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
        // ToDo: Test to see if the pod has collided with the gravitational pull of the planet.
        // This is basically if the pod has crossed the line that runs from the center of the planet perpendicular
        // to the pods forward vector.
        float dist = Vector3.Magnitude(pos - Center);
        Vector3 planetPosRelativeToPlayer = Game.Instance.Player.PlayerObject.transform.InverseTransformPoint(gameObject.transform.position);
        if(dist <= GravityRadius)
        {
            Logger.Log("PlanetRelative: " + planetPosRelativeToPlayer);
        }
        return (dist <= GravityRadius && planetPosRelativeToPlayer.y <= 0);
    }

}
