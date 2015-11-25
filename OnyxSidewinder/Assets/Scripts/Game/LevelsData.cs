using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelsData
{
	public List<LevelData> Levels;
	public LevelsData(){ Levels = new List<LevelData>(); }
}


[System.Serializable]
public class LevelData
{
	public int ID;
    public int Area;
    public int Order;
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
    public ScenesData() { Scenes = new List<SceneData>(); }
}

[System.Serializable]
public class SceneData
{
    public string Path;
    public string Name;
    public int Id;
    public SceneData() { }
    public SceneData(string name, int id)
    {
        Path = name;
        Id = id;
        Name = ParseName(name);
    }

    private string ParseName(string s)
    {
        // Remove ".unity"
        int length = s.Length;
        int typeIndex = s.IndexOf(".unity");
        if(typeIndex>0)
        {
            s = s.Substring(0, typeIndex);
        }

        // Remove Path Prefix
        int lastSlash = s.LastIndexOf("/");
        if(lastSlash>0)
        {
            s = s.Substring(lastSlash+1, s.Length - lastSlash - 1);
        }

        return s;
    }
}

public enum SceneType
{
    Level,
    UI,
    Main
}