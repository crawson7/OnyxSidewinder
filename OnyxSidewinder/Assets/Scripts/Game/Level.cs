using UnityEngine;
using System.Collections;

public class Level
{
	// State Machine
	private PlanetManager _planets;
	private LevelData _data;
    private Rect _bounds;
    private Vector3 _startPosition;

    // Planet generation
    private float _minGapDist;
    private float _maxGapDist;
    private float _minBodySize;
    private float _maxBodySize;
    private float _minGravityDepth;
    private float _maxGravityDepth;

    // World Settings
    private float _gravity;
    private float _rotationAcceleration;
    private float _equilibriumSpeed;
    private float _orbitAccelerationTime;
    private bool _dataLoaded = false;

    public PlanetManager Planets{get{return _planets;}}

	public bool Initialize(LevelData data)
	{
		// TODO:Load the scene specified for this level.

		return true;
	}

    public void SetLevelData(LevelSettingsEditor settings)
    {
        _bounds = settings.Bounds;
        _startPosition = settings.StartPosition;
        _minGapDist = settings.MinGapDist;
        _maxGapDist = settings.MaxGapDist;
        _minBodySize = settings.MinBodySize;
        _maxBodySize = settings.MaxBodySize;
        _minGravityDepth = settings.MinGravityDepth;
        _maxGravityDepth = settings.MaxGravityDepth;
        _gravity = settings.Gravity;
        _rotationAcceleration = settings.RotationAcceleration;
        _equilibriumSpeed = settings.EquilibriumSpeed;
        _orbitAccelerationTime = settings.OrbitAccelerationTime;
        _dataLoaded = true;
        BuildLevel();
    }

    public void BuildLevel()
    {
        // TODO: Generate Planets and Place Player
        // TODO: Tell Game that Level is Ready.
    }
}
