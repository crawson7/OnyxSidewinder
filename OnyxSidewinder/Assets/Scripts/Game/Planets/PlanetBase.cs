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

public class PlanetBase : MonoBehaviour
{
    #region Private Members
    protected PlanetType _type;
    protected float _bodyRadius;
    protected float _gravityRadius;
    protected float _gapRadius;
    protected bool _active;
    protected StateMachine _states;
    #endregion

    #region Public Variables and Properties
    public GameObject PlanetObject;
    public GameObject GravityObject;

    public bool Active { get { return _active; } }
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
            PlanetObject.transform.localScale = Vector3.one * value * 2.0f;
        }
    }

    public float GravityRadius
    {
        get
        {
            return _gravityRadius;
        }
        set
        {
            _gravityRadius = value;
            GravityObject.transform.localScale = Vector3.one * value * 2.0f;
        }
    }
    #endregion

    public virtual void Initialize(float body, float gravity, PlanetType type)
    {
        _active = true;
        Type = type;
        BodyRadius = body;
        GravityRadius = gravity;
    }

    public virtual void Reset()
    {
        _active = true;
    }

    public virtual void SetType(PlanetType type)
    {
        // Modify the behavior and skin to match the type
        // Set the Sprite to match.
        // Reset the animations and state
    }

    public virtual void HandleBodyCollide()
    {

    }

    public virtual void HandleGravityCollide()
    {


    }

    public virtual void HandleOrbit()
    {

    }
}

