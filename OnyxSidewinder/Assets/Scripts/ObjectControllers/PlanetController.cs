using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {

    public bool Active; // Determines if this planet is to be considered in the collisiton tests.

    public Vector3 Center { get { return gameObject.transform.position; } }
    public float BodyRadius { get; set; }
    public float GravityRadius { get; set; }
    private SpriteRenderer _renderer;
    private bool _orbiting = false;


    public void Initialize(float body, float gravity)
    {
        BodyRadius = body;
        GravityRadius = gravity;
        Active = true;
        _renderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = Game.Instance.Player.Position;
        if (_orbiting && Vector3.Magnitude(pos - Center) > GravityRadius) // if we're flagged as inside the radius but we're actually outside
        {
            _renderer.color = new Color(255f, 255f, 255f); // TODO: Call PassiveState()
            _orbiting = false; //correct the _orbiting value to reflect that we're outside the gravity radius
        }
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
        if(dist<=BodyRadius)
        {
            _renderer.color = new Color(255f, 0f, 0f); //TODO: Call OrbitingState()
            return true;
        }
        return false;
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
            _renderer.color = new Color(0f, 255f, 0f); //TODO: Call CollidedState()
            _orbiting = true;
        }
        return (dist <= GravityRadius && planetPosRelativeToPlayer.y <= 0);
    }

}
