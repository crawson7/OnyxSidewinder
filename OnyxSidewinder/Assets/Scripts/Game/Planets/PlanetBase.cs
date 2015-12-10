using UnityEngine;
using System.Collections.Generic;


public enum PlanetType
{
    Popper,
    Bouncer,
    Chomper,
    Poker,
    Accelerator,
    Globber,
    Attractor
}

[ExecuteInEditMode]
public class PlanetBase : MonoBehaviour
{


    #region Private Members
    private PlanetType _type;
    private float _bodyRadius;
    private float _gravityDepth;
    protected float _gapDepth;
    protected bool _active;
	protected bool _orbitActive;
    protected StateMachine _states;
    public GameObject PlanetObject;
    public GameObject GravityObject;
	private PlanetData _data;
    #endregion

    #region Public Variables and Properties
    // Properties
    public Vector3 Center { get { return gameObject.transform.position; } }
    public PlanetData Data {get{return _data;}}
    public bool Active { get { return _active; } set { _active = value; } }
    public PlanetType Type { get { return _type; } set { _type = value; SetType(_type); } }
    public string pState { get { return _states.CurrentState; } set { if (_states.IsValid(value)){ _states.SetTo(value); } } }
    public float BodyRadius
    {
        get
        {
            return _bodyRadius;
        }
        set
        {
            _bodyRadius = value;
            SetScale();
        }
    }

    public float GravityDepth
    {
        get
        {
            return _gravityDepth;
        }
        set
        {
            _gravityDepth = value;
            SetScale();
        }
    }
    #endregion

    public virtual void Initialize(PlanetData data)
    {
		PlanetObject = gameObject.transform.GetChild(0).gameObject;
		GravityObject = gameObject.transform.GetChild(1).gameObject;
		if(PlanetObject == null || GravityObject == null)
		{Logger.Log("Error Initializing Planet " + data.Type, 4);return;}

        _data = data;
        _active = true;
		_orbitActive = false;
        Type = data.Type;
        BodyRadius = data.Body;
        GravityDepth = data.Gravity;
    }

	private void Update()
	{
		if(_orbitActive)
		{
			HandleOrbit();
		}
	}

    public void Terminate()
    {
		Object.Destroy(this.gameObject);
    }

    public virtual void Reset()
    {
        _active = true;
    }

    public virtual void SetType(PlanetType type)
    {
        SpriteRenderer sr = PlanetObject.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + Global.GetPlanetTypeData(_data.Type).Sprite);

        // TODO: Modify the behavior and skin to match the type
    }

    public virtual void SetScale()
    {
        PlanetObject.transform.localScale = Vector3.one * BodyRadius * 2f;
        GravityObject.transform.localScale = Vector3.one * GravityDepth * 2f;
    }

    public bool TestCollision()
    {
        Vector3 pos = Game.Instance.Player.Position;
        if (BodyCollision(pos))
        {
            Logger.Log("Body Collide", 2);
            //Game.Instance.HandlePlanetCollide(this);
			HandleBodyCollide();
            return true;
        }
        else if (GravityCollision(pos))
        {
            Logger.Log("Gravity Collide", 2);
            //Game.Instance.HandleGravityCollide(this);
			HandleGravityCollide();
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
        return (dist <= BodyRadius);
    }

    public bool GravityCollision(Vector3 pos)
    {
        float dist = Vector3.Magnitude(pos - Center);
        Vector3 planetPosRelativeToPlayer = Game.Instance.Player.PlayerObject.transform.InverseTransformPoint(gameObject.transform.position);
        return (dist <= GravityDepth && planetPosRelativeToPlayer.z <= 0);
    }

    public virtual void HandleBodyCollide()
    {
		Game.Instance.HandlePlanetCollide(this);
    }

    public virtual void HandleGravityCollide()
    {
		_orbitActive = true;
		Game.Instance.HandleGravityCollide(this);
    }

	public virtual void Release()
	{
		_orbitActive = false;
		_active = false;
	}

    public virtual void HandleOrbit()
    {
		// Runs While Palyer is orbiting.
    }
}

