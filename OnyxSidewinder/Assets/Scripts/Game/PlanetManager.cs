using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetManager
{
    private List<PlanetController> _planets;
    private Bounds _bounds = new Bounds();
    private Rect _rect;
    private Vector3 _startPosition;
    private float _minGapDist;
    private float _maxGapDist;
    private float _minBodySize;
    private float _maxBodySize;
    private float _minGravityDepth;
    private float _maxGravityDepth;
    private List<PlanetBase> _newPlanets = new List<PlanetBase>();

	public bool Initialize(LevelSettingsEditor settings)
	{
        _rect = settings.Bounds;
        _startPosition = settings.StartPosition;
        _minGapDist = settings.MinGapDist;
        _maxGapDist = settings.MaxGapDist;
        _minBodySize = settings.MinBodySize;
        _maxBodySize = settings.MaxBodySize;
        _minGravityDepth = settings.MinGravityDepth;
        _maxGravityDepth = settings.MaxGravityDepth;

		LoadManualPlanets();
        return true;
	}

    public void RegisterPlanet(PlanetEditor planet)
    {
        Logger.Log("Planet Registered: Type=" + planet.Type + " Size=" + planet.PlanetRadius, 1);
		// TODO: Assign Behavior, Type and all other settings to the planet.
		// TODO; Store the planet in the planets registry.
    }

    public void BuildNewLevel()
    {
        // TODO: Destroy old level objects if needed.

        SceneBuilder sb = new SceneBuilder();
        _planets = sb.LoadPlanets(_rect, _startPosition, _minBodySize, _maxBodySize, _minGravityDepth, _maxGravityDepth, _minGapDist, _maxGapDist);
        if (_planets.Count == 0) { Logger.Log("Plant Loading Failed.", 5); return; }

    }

    public void Restart()
    {
        for (int i = 0; i < _planets.Count; i++)
        {
            _planets[i].Reset();
        }
    }

	private void LoadManualPlanets()
	{
		// Get all Manually placed planets in the Level Scene.
		SceneController sc = SceneManager.Instance.GetScene(Global.Level.Name);
		if(sc == null){return;}
		PlanetEditor[] manualPlanets = sc.gameObject.GetComponentsInChildren<PlanetEditor>(false);

		// Register each of the manually placed planets.
		for(int i=0; i<manualPlanets.Length; i++)
		{
			RegisterPlanet(manualPlanets[i]);
		}
	}

    public bool CheckShipAttach()
    {
        for (int i = 0; i < _planets.Count; i++)
        {
            if (!_planets[i].Active)
            {
                if (_planets[i].TestExit())
                {
                    _planets[i].Active = true;
                }
            }
            else
            {
                if (_planets[i].TestCollision())
                {
                    // Reset all planets to active state
                    for (int j = 0; j < _planets.Count; j++)
                    {
                        _planets[j].Active = true;
                    }
                    return true;
                }
            }
        }
        return false;
    }

}
