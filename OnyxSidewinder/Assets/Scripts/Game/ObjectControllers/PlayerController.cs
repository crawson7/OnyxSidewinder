using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private float _forwardSpeed;                    // Unity Units per second
    private float _rotationSpeed;                   // Euler Degrees of rotation per second
    private bool _orbiting;                         // If the player is currently orbiting a planet
    private bool _charging = false;
    private float _radius;
    private float _circumfrence;
    private PlanetBase _orbit;
    private bool _alive;
    private Vector2 _velocity;
    private float _gravity = 8.25f;
    private float _defaultSpeed = 5f;               // The players speed will adjust to match this orbit speed over time.
    private float _topSpeed = 15f;                  // The maximum orbit speed that the player will reach while holding down.
    private float _orbitStartSpeed;                 // The speed the player was going when he entered orbit.
    private float _orbitTime;                       // The amount of time the player has been in orbit on the active planet.
    private float _angleDirection;                  // The direction of rotation around the planet. >0 = Clockwise, <0 = Counter Clockwise
    private float _orbitAccelerationTime = 2.5f;    // The amount of time it takes for the players speed to reach the default speed.
    private float _chargeAccelerationTime = 3.5f;   // The amount of time it takes for the players speed to reach the top speed while charging up.
    private float _orbitForwardVelocity;            // The current forward speed of the players orbit.
    private Easing.Function _orbitEasing;           // The easing function to use for adjusting the players rotation speed in orbit.
    private Easing.Function _chargeEasing;          // The easing function to use for speeding the players orbit up while charging.
    private float _boostAmount = 6f;                // The amount of release boost as a percentage of Forward Velocity

    public GameObject PlayerObject;
    public GameObject Pivot;
    public GameObject ForwardIndicator;

    public Vector3 Forward { get { return new Vector3(0,1,0); } }
    public float Radius { get { return _radius; } set { _radius = value; } } // Distance between Pod and Planet.
    public Vector3 Position { get { return PlayerObject.transform.position; } } // Pod Position in World Space
    public bool Orbiting { get { return _orbiting; } }

    public float Speed {
        get
        {
            if (_orbiting)
                return _orbitForwardVelocity;
            else
                return _velocity.magnitude;
        }
    }

    public bool Initialize()
    {
        _orbiting = false;
        _alive = true;
        _orbitEasing = Easing.EaseFunction(EaseType.EaseInSine);
        _chargeEasing = Easing.EaseFunction(EaseType.Linear);
        ForwardIndicator.SetActive(false);
        return true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!_alive || Game.Instance.CurrentLevel.State != LevelState.Playing) { return; }
        float deltaTime = Time.deltaTime;

        if (_orbiting)
        {
            UpdateOrbit(deltaTime);
        }
        else
        {
            UpdatePosition(deltaTime);
        }
	}

    public void UpdateOrbit(float time)
    {
        // Continue orbitin player
        Pivot.transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));

        // Adjust rotation Speed based on orbit time
        _orbitTime += Time.deltaTime;
        _orbitForwardVelocity = (_charging)? _topSpeed : _defaultSpeed;
        if (_orbitTime < _orbitAccelerationTime)
        {
            float percentTime = _orbitTime / _orbitAccelerationTime;
            if(_charging)
            {
                float distance =  _topSpeed - _orbitStartSpeed;
                _orbitForwardVelocity = _chargeEasing(_orbitStartSpeed, distance, _orbitTime, _chargeAccelerationTime);
            }
            else
            {
                float distance = _defaultSpeed - _orbitStartSpeed;
                _orbitForwardVelocity = _orbitEasing(_orbitStartSpeed, distance, _orbitTime, _orbitAccelerationTime);
            }
        }
        _rotationSpeed = _orbitForwardVelocity / _circumfrence * 360 * _angleDirection;
    }
    
    public void Charge()
    {
        _orbitTime = 0;
        _charging = true;
        _orbitStartSpeed = _orbitForwardVelocity;
    }

    public void UpdatePosition(float time)
    {
        Vector2 position = PlayerObject.transform.position;
        Vector2 newPos, newVel;
        CalculatePositionUpdate(time, PlayerObject.transform.position, _velocity, out newPos, out newVel);

        PlayerObject.transform.position = newPos;
        _velocity = newVel;
        PlayerObject.transform.LookAt(position + _velocity, new Vector3(0, 0, -1));

        // Old Calculation
        //Vector2 position = PlayerObject.transform.position;
        //position += (_velocity * time) + (time * new Vector2(0, -_gravity) * 0.5f);
        //_velocity += new Vector2(0, time * -_gravity);
        //PlayerObject.transform.position = position;
        //PlayerObject.transform.LookAt(position + _velocity, new Vector3(0, 0, -1));
    }
    
    private void CalculatePositionUpdate(float deltaTime, Vector2 pos, Vector2 vel, out Vector2 newPos, out Vector2 newVel)
    {
        newPos = pos + (vel * deltaTime) + (deltaTime * new Vector2(0, -_gravity) * 0.5f);
        newVel = vel + new Vector2(0, deltaTime * -_gravity);
    }

    public void Kill()
    {
        _alive = false;
        PlayerObject.SetActive(false);

        Game.Instance.CurrentLevel.updateUIDistance("DEATH");
    }

    public void Reset(Vector3 pos)
    {
        _orbiting = false;
        _alive = true;
        _velocity = Vector2.zero;
        _rotationSpeed = 0;
        SetPivot(pos, false);
        PlayerObject.transform.localPosition = Vector3.zero;
        Pivot.transform.eulerAngles = Vector3.zero;
        PlayerObject.transform.eulerAngles = new Vector3(270, 0,0);
        PlayerObject.SetActive(true);
        
    }
    
    public void SetSpeed(float s)
    {
        _forwardSpeed = Mathf.Abs(s);
        _velocity = PlayerObject.transform.forward * _forwardSpeed;
    }

    public void ChangeRotation(float degrees)
    {
        Pivot.transform.Rotate(new Vector3(0, 0, degrees));
    }

	public void Reflect()
	{
		// TODO: reverse the players velocity but to not let the player attach to the current planet.
	}

    public void Orbit(PlanetBase planet)
    {
		if(planet.Center.y > Game.Instance.GoalHeight)
		{
			Game.Instance.HandleReachGoal();
			return;
		}
        Logger.Log("Orbit planet center at " + planet.Center, 2);
        _orbit = planet;
        Orbit(planet.Center, false);
    }

    public void Orbit(Vector3 pos, bool relative)
    {
        Vector3 worldPos = Vector3.zero;
        if(relative)
        {
            worldPos = PlayerObject.transform.TransformPoint(pos);
        }
        else
        {
            worldPos = pos;
        }
        SetPivot(worldPos, true);
        Radius = Vector3.Magnitude(worldPos - PlayerObject.transform.position);
        _circumfrence = Mathf.PI * Radius * 2.0f;
        Vector3 planetPosRelativeToPlayer = PlayerObject.transform.InverseTransformPoint(_orbit.transform.position);
        _angleDirection = -Utilities.AngleDirection2(Forward, planetPosRelativeToPlayer);
        _orbitStartSpeed = _velocity.magnitude;
        _rotationSpeed = _velocity.magnitude / _circumfrence * 360 * _angleDirection;
        _orbitTime = 0;
        _orbiting = true;
    }

    public void Release()
    {
        float speedBoost = _boostAmount * _angleDirection;
        SetSpeed(_circumfrence * (_rotationSpeed * 0.00278f) + speedBoost);
        _orbiting = false;
        _charging = false;
    }

    private void SetPivot(Vector3 pos, bool preserveLocation)
    {
        Vector3 difference = pos - Pivot.transform.position;
        Vector3 childOrigin = PlayerObject.transform.position;
        Pivot.transform.position = Pivot.transform.position + difference;
        PlayerObject.transform.position = childOrigin;
    }

    public void ShowForwardVector()
    {
        ForwardIndicator.SetActive(true);
        LineRenderer lr = ForwardIndicator.GetComponent<LineRenderer>();
        lr.SetVertexCount(10);
        Vector2 pos = PlayerObject.transform.position;
        Vector2 vel = PlayerObject.transform.forward * 14.0f;
        float timeIncrement = 0.015f;
        for (int i=0; i<10; i++)
        {
            Vector2 newPos, newVel;
            CalculatePositionUpdate(timeIncrement * i, pos, vel, out newPos, out newVel);
            lr.SetPosition(i, newPos);
            vel = newVel;
            pos = newPos;
        }
    }

    public void HideForwardVector()
    {
        ForwardIndicator.SetActive(false);
    }
}
