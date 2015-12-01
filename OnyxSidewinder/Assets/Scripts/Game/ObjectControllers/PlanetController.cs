using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {
    public GameObject PlanetObject;
    public GameObject GravityObject;
    public bool Active; // Determines if this planet is to be considered in the collisiton tests.
	public bool full = false;
	public int ID;

    public Vector3 Center { get { return gameObject.transform.position; } }
    public float BodyRadius { get; set; } // the distance from the center to the edge of the body mass of the planet.
    public float GravityRadius { get; set; } // the distance from the center to the edge of the plants gravity field
	public float GapRadius {get; set; } // The distance from the center to the jump gap edge, used for planet generation


    public void Initialize(float body, float gravity, PlanetType type)
    {
        Active = true;
        SetBodySize(body);
        SetGravitySize(gravity);
        SetSprite();
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
		Active = true;
    }
    
    private void SetSprite()
    {
        if(BodyRadius>1.25)
        {
            PlanetObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/big_guy");
        }
        else if(BodyRadius>0.75)
        {
            PlanetObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/flapper");
        }
        else
        {
            PlanetObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/lil_guy");
        }
    }

    public bool TestCollision()
    {
        Vector3 pos = Game.Instance.Player.Position;
        if (BodyCollision(pos))
        {
            Logger.Log("Body Collide", 2);
            Game.Instance.HandlePlanetCollide(this);
            return true;
        }
        else if (GravityCollision(pos))
        {
            Logger.Log("Gravity Collide", 2);
            Game.Instance.HandleGravityCollide(this);
            return true;
        }
        return false;
    }

	public bool TestExit()
	{
		return !GravityCollision(Game.Instance.Player.Position);
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
