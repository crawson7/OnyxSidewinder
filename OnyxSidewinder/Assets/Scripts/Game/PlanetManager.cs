using UnityEngine;
using System.Text;
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
    private List<PlanetData> _planetsData = new List<PlanetData>();

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
        PlanetData pd = new PlanetData(planet.Type, planet.PlanetObject.transform.position, 0f, planet.GravityDepth, planet.PlanetRadius);
        _planetsData.Add(pd);
        planet.Terminate();
    }

    public void BuildNewLevel()
    {
        SceneBuilder sb = new SceneBuilder();
        _planetsData.AddRange( sb.LoadPlanets(_rect, _startPosition, _minBodySize, _maxBodySize, _minGravityDepth, _maxGravityDepth, _minGapDist, _maxGapDist));
        //if (_planets.Count == 0) { Logger.Log("Plant Loading Failed.", 5); return; }
        PrintPlanetData();
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
        PrintPlanetData();
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
