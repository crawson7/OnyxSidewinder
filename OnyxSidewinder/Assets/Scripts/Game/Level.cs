using UnityEngine;
using System.Collections;

public class Level
{
	// State Machine
	private PlanetManager _planets;
	private LevelData _data;

	public PlanetManager Planets{get{return _planets;}}

	public bool Initialize(LevelData data)
	{
		// Initialize Level with data
		_planets = new PlanetManager();
		if(!_planets.Initialize(data.Planets)){return false;}

		return true;
	}
}
