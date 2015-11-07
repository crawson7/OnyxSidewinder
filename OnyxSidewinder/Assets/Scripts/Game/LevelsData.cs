using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelsData
{
	public List<LevelData> Levels;
	public LevelsData(){}
}


[System.Serializable]
public class LevelData
{
	public int ID;
	public PlanetsData Planets;
	public LevelData(){}
}

[System.Serializable]
public class PlanetsData
{
	public int ID;
	public PlanetsData(){}
}