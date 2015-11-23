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
    public string SceneName; 

    public PlanetsData Planets;
	public LevelData(){}
}

[System.Serializable]
public class PlanetsData
{
	public int ID;
	public PlanetsData(){}
}

[System.Serializable]
public class ScenesData
{
    public List<SceneData> Scenes;
    public ScenesData() { }
}

[System.Serializable]
public class SceneData
{
    public string Name;
    public int Id;
    public SceneType Type;
    public SceneData() { }
}

public enum SceneType
{
    Level,
    UI,
    Main
}