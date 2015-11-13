using UnityEngine;
using System.Collections;

public class LevelSettingsEditor : MonoBehaviour
{
    // Planet Generation Settings
    [Header("Planet Generation")]
    public float MinGapDist = 0.75f;
    public float MaxGapDist = 2.0f;
    public float MinBodySize = 0.3f;
    public float MaxBodySize = 1.5f;
    public float MinGravityDepth = 1.5f;
    public float MaxGravityDepth = 3.0f;

    // World Settings
    [Header("World Settings")]
    public float Gravity = 3.5f;
    public float RotationAcceleration = 3.0f;
    public float EquilibriumSpeed = 10f;
    public float OrbitAccelerationTime = 3.0f;

    // Player Settings

    private GameObject _levelBounds;
    private GameObject _player;

    public Rect Bounds
    {
        get
        {
            LevelBoundsIndicator bounds = _levelBounds.GetComponent<LevelBoundsIndicator>();
            return new Rect(0, bounds.Height * 0.5f, bounds.Width, bounds.Height);
        }
    }

    public Vector3 StartPosition
    {
        get
        {
            return _player.transform.position;
        }
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Initialize(GameObject bounds, GameObject player)
    {
        _levelBounds = bounds;
        _player = player;
    }
}
