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
    protected StateMachine _states;
    protected GameObject _planetObject;
    protected GameObject _gravityObject;
    #endregion

    #region Public Variables and Properties
    // Properties
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

    public virtual void Initialize(float body, float gravity, PlanetType type)
    {
        _active = true;
        Type = type;
        BodyRadius = body;
        GravityDepth = gravity;
    }

    public virtual void Reset()
    {
        _active = true;
    }

    public virtual void SetType(PlanetType type)
    {
        //sr.sprite = Resources.Load<Sprite>("Sprites/" + data.Sprite);
        // Modify the behavior and skin to match the type
        // Set the Sprite to match.
        // Reset the animations and state
    }

    public virtual void SetScale()
    {

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

