using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private float _forwardSpeed; // Unity Units per second
    private float _rotationSpeed; // Euler Degrees per second
    private bool _orbiting;
    private float _radius;
    private float _circumfrence;
    private PlanetController _orbit;
    private bool _alive;
    private Vector2 _velocity;
    private float _gravity = 6.0f;
    private float _rotationAcceleration = 3.0f;
    private float _topSpeed = 15;
    private float _orbitStartSpeed;
    private float _orbitTime;
    private float _angleDirection;
    private float _orbitAccelerationTime = 3.0f;
    
    public GameObject PlayerObject;
    public GameObject Pivot;

    public Vector3 Forward { get { return new Vector3(0,1,0); } }
    public float Radius { get { return _radius; } set { _radius = value; } } // Distance between Pod and Planet.
    public Vector3 Position { get { return PlayerObject.transform.position; } } // Pod Position in World Space
    public bool Orbiting { get { return _orbiting; } }

    public bool Initialize()
    {
        _orbiting = false;
        _alive = true;
        return true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!_alive || !Game.Instance.LevelActive) { return; }

        if (_orbiting)
        {
            UpdateOrbit();
        }
        else
        {
            UpdatePosition();
        }
	}

    public void UpdateOrbit()
    {
        Pivot.transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime)); 
        _orbitTime += Time.deltaTime;
        float percentTime = _orbitTime / _orbitAccelerationTime;
        percentTime = (percentTime>1.0f)? 1.0f : percentTime;
        float currentSpeed = _orbitStartSpeed + ((_topSpeed - _orbitStartSpeed) * percentTime);
        _rotationSpeed = currentSpeed / _circumfrence * 360 * _angleDirection;
    }
    
    public void UpdatePosition()
    {
        Vector2 position = PlayerObject.transform.position;
        PlayerObject.transform.LookAt(position + _velocity, new Vector3(0,0,-1));
        position += (_velocity * Time.deltaTime) + (Time.deltaTime * new Vector2(0, -_gravity) * 0.5f);
        _velocity += new Vector2(0, Time.deltaTime * -_gravity);
        PlayerObject.transform.position = position;
    }
    
    public void Kill()
    {
        _alive = false;
        PlayerObject.SetActive(false);
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

    public void Orbit(PlanetController planet)
    {
        Logger.Log("Orbit planet center at " + planet.Center);
        _orbit = planet;
        _orbit.Active = false;
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
        SetSpeed(_circumfrence * (_rotationSpeed * 0.00278f));
        _orbiting = false;
    }

    private void SetPivot(Vector3 pos, bool preserveLocation)
    {
        Vector3 difference = pos - Pivot.transform.position;
        Vector3 childOrigin = PlayerObject.transform.position;
        Pivot.transform.position = Pivot.transform.position + difference;
        PlayerObject.transform.position = childOrigin;
    }


}
