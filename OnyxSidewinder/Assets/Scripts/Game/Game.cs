using UnityEngine;
using System.Collections.Generic;
using System;

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
    private float _checkInterval;
    private float _checkFrequency = 0.1f;
    private List<PlanetController> _planets = new List<PlanetController>();
    private bool _active;
    private OnyxUIController _uicontrol;

    public GameObject GameObj;
    public GameObject DialogsObj;
	private Bounds _bounds = new Bounds();

    public bool Touching;

    public PlayerController Player { get { return _player; } }
    public bool LevelActive {get{return _playing;}}

    public bool Initialize()
    {
        GameObj = GameObject.Find("Game");
        DialogsObj = GameObject.Find("Dialogs");
        if(GameObj == null || DialogsObj == null) { Logger.Log("Could not find parent objects"); return false; }

        if (!LoadPlayer()) { return false; }
        if (!LoadPlanets()) { return false; }
        if (!LoadUI()) { return false; }
        Logger.Log("Game System Initialized.");
		LoadBoundries();
		_active = true;
        _playing = false;
        return true;
    }

    public void Restart()
    {
        _player.Reset(new Vector3(2, 0,0));
        for(int i=0; i<_planets.Count; i++)
        {
            _planets[i].Reset();
        }
        _active = true;
    }
    
    public void Update()
    {
        if(!_active) { return; }

        _checkInterval += Time.deltaTime;
        if (_checkInterval >= _checkFrequency)
        {
            if (!_player.Orbiting)
            { 
				if(CheckLostInSpace()){return;}
				if(CheckShipAttach()){return;} 
			}
        }
    }

	private bool CheckLostInSpace()
	{
		if(_bounds.Contains(_player.Position))
		{return false;}
		HandleLostInSpace();
		return true;
	}

    private bool CheckShipAttach()
    {
        for (int i = 0; i < _planets.Count; i++)
        {
            if (!_planets[i].Active) { continue; }
            if (_planets[i].TestCollision())
            {
                return true;
            }
        }
		return false;
    }

    public bool LoadUI()
    {
        GameObject prefab = Resources.Load("UI/OnyxUI") as GameObject;
        GameObject onyxUI = GameObject.Instantiate<GameObject>(prefab);
        _uicontrol = onyxUI.GetComponent<OnyxUIController>();
        if(!_uicontrol.Initialize()) { Logger.Log("UI did not initialize properly"); return false; }

        return true;
    }


    public bool LoadPlayer()
    {
        GameObject prefab = Resources.Load("Game/Player") as GameObject;
        GameObject player = GameObject.Instantiate<GameObject>(prefab);
        _player = player.GetComponent<PlayerController>();
        _player.gameObject.transform.position = new Vector3(0, -8, 0);
        _player.gameObject.transform.SetParent(GameObj.transform, false);
        if (!_player.Initialize()) { Logger.Log("Player did not initialize propperly"); return false; }
        return true;
    }
    /*
    public bool LoadPlanets()
    {
        // This will all eventually be either procedural or from data.
        GameObject prefab = Resources.Load("Game/Planet") as GameObject;

        GameObject planet1 = GameObject.Instantiate<GameObject>(prefab);
        if(planet1 == null) { return false; }
        PlanetController pc = planet1.GetComponent<PlanetController>();
        pc.Initialize(0.6f, 3.5f);
        _planets.Add(pc);
        pc = planet1.GetComponent<PlanetController>();
        pc.gameObject.transform.position = new Vector3(3.5f, 6f, 0);
        pc.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet2 = GameObject.Instantiate<GameObject>(prefab);
        if (planet2 == null) { return false; }
        PlanetController pc2 = planet2.GetComponent<PlanetController>();
        pc2.Initialize(0.4f, 3.5f);
        _planets.Add(pc2);
        pc2 = planet2.GetComponent<PlanetController>();
        pc2.gameObject.transform.position = new Vector3(-3.5f, 6f, 0);
        pc2.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet3 = GameObject.Instantiate<GameObject>(prefab);
        if (planet3 == null) { return false; }
        PlanetController pc3 = planet3.GetComponent<PlanetController>();
        pc3.Initialize(0.8f, 3.5f);
        _planets.Add(pc3);
        pc3 = planet3.GetComponent<PlanetController>();
        pc3.gameObject.transform.position = new Vector3(3.5f, -6f, 0);
        pc3.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet4 = GameObject.Instantiate<GameObject>(prefab);
        if (planet4 == null) { return false; }
        PlanetController pc4 = planet4.GetComponent<PlanetController>();
        pc4.Initialize(0.2f, 3.5f);
        _planets.Add(pc4);
        pc4 = planet4.GetComponent<PlanetController>();
        pc4.gameObject.transform.position = new Vector3(-3.5f, -6f, 0);
        pc4.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet5 = GameObject.Instantiate<GameObject>(prefab);
        if (planet5 == null) { return false; }
        PlanetController pc5 = planet5.GetComponent<PlanetController>();
        pc5.Initialize(0.2f, 3.5f);
        _planets.Add(pc5);
        pc5 = planet5.GetComponent<PlanetController>();
        pc5.gameObject.transform.position = new Vector3(0f, 0f, 0);
        pc5.gameObject.transform.SetParent(GameObj.transform, false);

        return true;
    }*/

    float startHeight = 0;
    float goalHeight = 50;
    float levelWidth = 15;
    float minJumpDist = 5.0f;
    float maxJumpDist = 8.0f;
    float minGapAngle = 90.0f;
    float minBodySize = 0.1f;
    float maxBodySize = 0.5f;
    float minGravityDepth = 2.0f;
    float maxGravityDepth = 3.5f;

    public bool LoadPlanets()
    {
        // Set Bounds
        _bounds.center = new Vector3(0, 25, 0);
        _bounds.extents = new Vector3(8, 55, 0);

        // Place Planets
        PlanetController pc = PlacePlanet(0.2f, 2.0f, new Vector3(0, 4, 0));
        if(pc== null) { return false; }

        PlanetController next = GetNextBranch();
        int count = 0;
        while (next != null && count <3)
        {
            Branch(next);
            next = GetNextBranch();
            count++;
        }
        return true;
    }

    private PlanetController GetNextBranch()
    {
        // Get the next planet to branch from, Could be the same as current planet.
        PlanetController next = _planets[_planets.Count - 1];
        if (next.Center.y >= goalHeight) { return null; }
        return next;
    }

    private void Branch(PlanetController pc)
    {
        //GameObject tempGo = new GameObject("temp");
        //Transform t = tempGo.transform;

        float newPlanetRadius = 2.0f;

        // Can be placed directly left? 
        float spaceBetweenPlanetAndBounds = pc.Center.x - pc.GravityRadius - _bounds.min.x;
        if (spaceBetweenPlanetAndBounds >= newPlanetRadius * 2)
        {
            //Place new planet directly to the left.
            PlacePlanet(0.5f, newPlanetRadius, pc.Center - new Vector3(pc.GravityRadius + newPlanetRadius, 0,0));
            return;
        }

        float newXpos = _bounds.min.x + newPlanetRadius;
        float x = pc.Center.x - newXpos;
        float h = pc.GravityRadius + newPlanetRadius;
        float newYpos = (Mathf.Sqrt((h * h) - (x * x)) + pc.Center.y);
        PlacePlanet(0.5f, newPlanetRadius, new Vector3(newXpos, newYpos, 0));

        /*
        t.position = pc.Center;
        t.forward = new Vector3(0, 1, 0);
        float targetangle = UnityEngine.Random.Range(240.0f, 120.0f);
        float targetDistance = UnityEngine.Random.Range(minJumpDist, maxJumpDist);
        t.Rotate(t.forward, targetangle);
        t.Translate(t.up * targetDistance);
        float bodySize = UnityEngine.Random.Range(minBodySize, maxBodySize);
        float gravityDepth = UnityEngine.Random.Range(minGravityDepth, maxGravityDepth);
        float gravitySize = bodySize + gravityDepth;
        PlacePlanet(bodySize, gravitySize, t.position);
        UnityEngine.Object.Destroy(t.gameObject);
        */
    }

    private PlanetController PlacePlanet(float body, float gravity, Vector3 pos)
    {
        GameObject prefab = Resources.Load("Game/Planet") as GameObject;

        GameObject planet5 = GameObject.Instantiate<GameObject>(prefab);
        if (planet5 == null) { return null; }
        PlanetController pc5 = planet5.GetComponent<PlanetController>();
        pc5.Initialize(body, gravity);
        _planets.Add(pc5);
        pc5 = planet5.GetComponent<PlanetController>();
        pc5.gameObject.transform.position = pos;
        pc5.gameObject.transform.SetParent(GameObj.transform, false);
        return pc5;
    }


	public void LoadBoundries()
	{
		if(_planets.Count <= 0){return;}
		float leftEdge = _planets[0].Center.x - _planets[0].GravityRadius;
		float rightEdge = _planets[0].Center.x + _planets[0].GravityRadius;
		float bottomEdge = _planets[0].Center.y - _planets[0].GravityRadius;
		float topEdge = _planets[0].Center.y + _planets[0].GravityRadius;
		for(int i=0; i<_planets.Count; i++)
		{
			PlanetController pc = _planets[i];
			if(pc.Center.x - pc.GravityRadius < leftEdge)
			{leftEdge = pc.Center.x - pc.GravityRadius;}

			if(pc.Center.x + pc.GravityRadius > rightEdge)
			{rightEdge = pc.Center.x + pc.GravityRadius;}

			if(pc.Center.y - pc.GravityRadius < bottomEdge)
			{bottomEdge = pc.Center.y - pc.GravityRadius;}

			if(pc.Center.y + pc.GravityRadius > topEdge)
			{topEdge = pc.Center.y + pc.GravityRadius;}
		}	

		topEdge += 5.0f; // Add a buffer at the top.
        bottomEdge += -3; // Buffer at the bottom.
		Vector3 center = new Vector3((rightEdge + leftEdge)*0.5f, (topEdge + bottomEdge)*0.5f, 0);
		Vector3 extents = new Vector3((rightEdge - leftEdge)*0.5f, (topEdge - bottomEdge)*0.5f, 1);
		_bounds.center = center;
		_bounds.extents = extents;
		Logger.Log("Game Bounds = Center: " + _bounds.center + " Extents: " + _bounds.extents);
	}

    public void Start()
    {
        //_player.SetSpeed(12.0f);
    }

    public void HandleTouch(Vector2 pos)
    {
        if(_player.Orbiting)
        {
            _player.Release();
        }
        else
        {
            _playing = true;
            _player.SetSpeed(12.0f);
        }
    }

    public void HandleRelease(Vector2 pos)
    {
        // Do nothing for now.
    }

    public void HandlePlanetCollide(PlanetController planet)
    {
        _player.Kill();
        End(false);
        _uicontrol.setMessageText("yer dead!");
    }

    public void HandleGravityCollide(PlanetController planet)
    {
        for(int i=0; i<_planets.Count; i++)
        {
            _planets[i].Active = true;
        }
        _player.Orbit(planet);

        _uicontrol.setMessageText(planet.Center.y.ToString("0"));
    }

	public void HandleLostInSpace()
	{
		_player.Kill();
		End(false);
	}


    public void End(bool win)
    {
        _active = false;
        _playing = false;
        Logger.Log("GAME OVER");
        Restart();
    }

}
