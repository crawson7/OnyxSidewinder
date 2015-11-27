using UnityEngine;
using System.Collections.Generic;
using System;

public enum GameState
{
    Active,
    Inactive
}

public class Game
{
    #region Singleton Access
    public static Game _instance;
    public static Game Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new Game();
            }
            return _instance;
        }
    }
    public Game() { }
    #endregion

    private PlayerController _player;
    private PlanetController _activePlanet;
    private float _checkInterval;
    private float _checkFrequency = 0.1f;
    public GameObject GameObj;
    public GameObject DialogsObj;
	private Level _level;
    private int _jumps = 0;
    private GameState _state;
	private LevelsData _levels;

	// Public Menbers
    public bool Touching;
    public GameState State { get { return _state; } }
    public PlayerController Player { get { return _player; } }
    public PlanetController ActivePlanet {  get { return _activePlanet; } }
	public Level CurrentLevel{get{return _level;}}
	public float GoalHeight {get{return CurrentLevel.Bounds.max.y;}}

    public bool Initialize()
    {
        _state = GameState.Inactive;
        GameObj = GameObject.Find("Game");
        DialogsObj = GameObject.Find("Dialogs");
        if(GameObj == null || DialogsObj == null) { Logger.Log("Could not find parent objects", 5); return false; }

        LoadLevelData();
        LoadSaveData();
        return true;
    }

    public void Restart()
    {
        _player.Reset(new Vector3(2f, 1f,0));
        CurrentLevel.Resart();
        _jumps = 0;
    }
    
    public void Update()
    {
        if (_state == GameState.Inactive || CurrentLevel.State != LevelState.Playing) { return; }

        _checkInterval += Time.deltaTime;
        if (_checkInterval >= _checkFrequency)
        {
              if (!_player.Orbiting)
              { 
            	if(CheckLostInSpace()){return;}
            	if(CurrentLevel.Planets.CheckShipAttach()){return;}
              }
        } 
    }

    public bool LoadSaveData()
    {
        // TODO: Load Save Data
        return true;
    }
    
    public void LoadLevelData()
    {
		_levels = new LevelsData();
        DataManager.Load(out _levels);
    }

    public void Start(string startSceneName)
    {
        SceneManager.Instance.LoadScene(startSceneName, () => 
        {
            Logger.Log("StartGame", 1);
        });
    }
    
    public void LoadLevelByOrder(int area, int order)
	{
		for(int i=0; i<_levels.Levels.Count; i++)
		{
			if(_levels.Levels[i].Area == area && _levels.Levels[i].Order == order)
			{
				LoadLevel(_levels.Levels[i]);
                return;
			}
		}
		Logger.Log("Requested Level " + area + ":" + order + " Not Found", 4);
	}

	public void LoadLevelById(int guid)
	{
		// TODO: look up level by GUID
	}

	public void LoadLevel(LevelData level)
	{
		//Unload Active Scenes.
		SceneManager.Instance.UnloadAllScenes();

		_level = new Level();
		if(!CurrentLevel.Load(level))
		{ Logger.Log("Level " + level.ID + " Failed to Load.", 5); }
		_state = GameState.Active;
	}

	private bool CheckLostInSpace()
	{
		if(CurrentLevel.Bounds.Contains(_player.Position))
		{return false;}
		HandleLostInSpace();
		return true;
	}


    public bool LoadPlayer(Vector3 startPos)
    {
        GameObject prefab = Resources.Load("Game/Player") as GameObject;
        GameObject player = GameObject.Instantiate<GameObject>(prefab);
        _player = player.GetComponent<PlayerController>();
        _player.gameObject.transform.SetParent(GameObj.transform, false);
       startPos = new Vector3(2, 0.5f, 0);
        if (!_player.Initialize()) { Logger.Log("Player did not initialize properly", 5); return false; }
        _player.Reset(startPos);
        return true;
    }

    public void HandleTouch(Vector2 pos)
    {
        if(_state != GameState.Active) { return; }

        if (CurrentLevel.State == LevelState.Playing && _player.Orbiting)
        {
            _player.Charge();
        }
    }

    public void HandleRelease(Vector2 pos)
    {
        if (CurrentLevel.State == LevelState.Stopped)
        {
            CurrentLevel.Start();
            _player.SetSpeed(14.0f);
            Player.HideForwardVector();
        }
        else if (CurrentLevel.State == LevelState.Playing)
        {
            if (_player.Orbiting)
            {
                ActivePlanet.Active = false;
                _player.Release();
            }
        }
    }

    public void HandleDrag(Vector2 delta, Vector2 pos)
    {
        if(CurrentLevel !=null && CurrentLevel.State == LevelState.Stopped)
        {
            float rotation = delta.x * 0.5f;
            Player.ChangeRotation(rotation);
            Player.ShowForwardVector();
        }
    }

    public void HandlePlanetCollide(PlanetController planet)
    {
        _player.Kill();
        End(false);
        //UI Death Message
    }

    public void HandleGravityCollide(PlanetController planet)
    {
        _player.Orbit(planet);
        _activePlanet = planet;
        _jumps++;

        CurrentLevel.updateUIJumps(_jumps.ToString());
        CurrentLevel.updateUIDistance(planet.Center.y.ToString("0"));
    }

	public void HandleLostInSpace()
	{
		_player.Kill();
		End(false);
	}

	public void HandleReachGoal()
	{
		_player.Kill();
		End(true);
	}

	public void End(bool win)
    {
		if(win)
		{
			Logger.Log("You Win!", 2);
		}
		else
		{
			Logger.Log("GAME OVER. You Lost.", 2);
		}
        Restart();
    }

}


public class Circle
{
	public Vector2 Center;
	public float Radius;

	public Circle(Vector2 center, float radius)
	{
		Center = center;
		Radius = radius;
	}
}

[System.Serializable]
public class PlanetData
{
	public Vector2 Pos;
	public float Gap, Gravity, Body;

	public PlanetData() {}

	public PlanetData(float gap, float gravity, float body)
	{
		Gap = gap;
		Gravity = gravity;
		Body = body;
		Pos = Vector2.zero;
	}
}