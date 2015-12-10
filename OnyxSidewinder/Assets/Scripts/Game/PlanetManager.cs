using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class PlanetManager
{
    private Bounds _bounds = new Bounds();
    private Rect _rect;
    private Vector3 _startPosition;
    private float _minGapDist;
    private float _maxGapDist;
    private float _minBodySize;
    private float _maxBodySize;
    private float _minGravityDepth;
    private float _maxGravityDepth;
    private List<PlanetData> _planetsData = new List<PlanetData>();
	private List<PlanetBase> _planets = new List<PlanetBase>();
    private GameObject _planetPrefab;

	#region INITIALIZATION AND DATA PREP
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
        _planetPrefab = Resources.Load("Game/Planet") as GameObject;
		LoadManualPlanets();
        return true;
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
        PrintPlanetData();
	}

	public void RegisterPlanet(PlanetEditor planet)
	{
		PlanetData pd = new PlanetData(planet.Type, planet.PlanetObject.transform.position, 0f, planet.GravityDepth, planet.PlanetRadius);
		_planetsData.Add(pd);
		planet.Terminate();
	}

	public void PrepNewLevel()
	{
		PlanetGenerator sb = new PlanetGenerator();
		_planetsData.AddRange( sb.LoadPlanets(_rect, _startPosition, _minBodySize, _maxBodySize, _minGravityDepth, _maxGravityDepth, _minGapDist, _maxGapDist));

		PrintPlanetData();
	}

	public void Restart()
	{
		for (int i = 0; i < _planets.Count; i++)
		{
			_planets[i].Reset();
		}
	}
	#endregion

	#region PLANET CREATION AND DESTRUCTION
	public void RefreshPlanets()
	{
        // Place any planets that are now in range
		for(int i=0; i<_planetsData.Count; i++)
		{
			if(!_planetsData[i].Placed && PlanetIsInRange(_planetsData[i]))
			{
				PlacePlanet(_planetsData[i]);
			}
		}

        // Remove Any planets that are no longer in Range
        List<int> indicesToRemove = new List<int>();
		for(int i=0; i<_planets.Count; i++)
		{
			if(!PlanetIsInRange(_planets[i].Data))
			{
                indicesToRemove.Add(i);
			}
		}
        for(int i=indicesToRemove.Count-1; i>=0; i--)
        {
            RemovePlanetAt(indicesToRemove[i]);
        }
	}

	private void PlacePlanet(PlanetData data)
	{
        GameObject go = (GameObject) GameObject.Instantiate(_planetPrefab, data.Pos, Quaternion.identity);
        if (go == null) { Logger.Log("There is an error Instantiating Planet", 4); return; }

        go.name = data.Type.ToString();
        go.transform.SetParent(Global.World.transform, false);

		// Attach Behavior
		PlanetBase planet = AttachPlanetBehavior(go, data.Type);
        if(planet== null) { Logger.Log("There is an error Instantiating Planet", 4);  return; }

        planet.Initialize(data);
        data.Placed = true;
        _planets.Add(planet);
	}

	private void RemovePlanetAt(int index)
	{
        _planets[index].Data.Placed = false;
        _planets[index].Terminate();
        _planets.RemoveAt(index);
        return;
	}

	public void RemovePlanet(PlanetBase planet)
	{
		for(int i=0; i<_planets.Count; i++)
		{
			if(_planets[i] == planet)
			{
				RemovePlanetAt(i);
			}
		}
		
	}

	private PlanetBase AttachPlanetBehavior(GameObject go, PlanetType type)
	{
		PlanetBase behavior;
		switch(type)
		{
			case PlanetType.Popper:
			{
				behavior = go.AddComponent<PlanetPopper>();
				break;
			}
			case PlanetType.Bouncer:
			{
				behavior = go.AddComponent<PlanetBouncer>();
				break;
			}
			case PlanetType.Chomper:
			{
				behavior = go.AddComponent<PlanetChomper>();
				break;
			}
			default:
			{
				behavior = go.AddComponent<PlanetBase>();
				break;
			}
		}
		return behavior;
	}

	private bool PlanetIsInRange(PlanetData data)
	{
        // TODO: Need to determine what constitutes a planet being in range.
		return true;
	}
	#endregion


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

    public void PrintPlanetData()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Planet Data:\n");
        for (int i=0; i<_planetsData.Count; i++)
        {
            builder.Append(_planetsData[i].ToString() + "\n");
        }
        Logger.Log(builder.ToString(), 1);
    }
}
