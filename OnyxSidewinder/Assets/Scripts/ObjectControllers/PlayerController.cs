using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private float _forwardSpeed; // Unity Units per second
    private float _rotationSpeed; // Euler Degrees per second
    private bool _orbiting;
    private float _radius;
    private PlanetController _orbit;

    public GameObject PlayerObject;
    public GameObject Pivot;

    public Vector3 Forward { get { return new Vector3(0,1,0)/*PlayerObject.transform.up*/; } }
    public float Radius { get { return _radius; } set { _radius = value; } } // Distance between Pod and Planet.
    public Vector3 Position { get { return PlayerObject.transform.position; } } // Pod Position in World Space
    public bool Orbiting { get { return _orbiting; } }

    public bool Initialize()
    {
        _orbiting = false;
        return true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_orbiting)
        {
            Pivot.transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
            Logger.Log("Rotation Speed: " + _rotationSpeed + " Angle: " + Pivot.transform.eulerAngles.z + " - Time: " + Time.time);
        }
        else
        {
            PlayerObject.transform.Translate(new Vector3(0, _forwardSpeed * Time.deltaTime, 0), Space.Self);
        }
	}

    public void Kill()
    {
        // Kill the player
    }

    public void SetSpeed(float s)
    {
        _forwardSpeed = Mathf.Abs(s);
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
        float circumfrence = Mathf.PI * Radius * 2.0f;
        Vector3 planetPosRelativeToPlayer = PlayerObject.transform.InverseTransformPoint(_orbit.transform.position);
        _rotationSpeed = _forwardSpeed / circumfrence * 360 * -Utilities.AngleDirection2(Forward, planetPosRelativeToPlayer);
        //SetSpeed(0);
        _orbiting = true;
    }

    public void Release()
    {
        float circumfrence = Mathf.PI * Radius * 2.0f;
        SetSpeed(circumfrence * (_rotationSpeed * 0.00278f));
        //_rotationSpeed = 0;
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
