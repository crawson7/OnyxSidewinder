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
    private PlanetController _activePlanet;
    private float _checkInterval;
    private float _checkFrequency = 0.1f;
    private List<PlanetController> _planets = new List<PlanetController>();
    private bool _active; // The Game is running
    private OnyxUIController _uicontrol;
    private bool _playing = false;
    public GameObject GameObj;
    public GameObject DialogsObj;
	private Bounds _bounds = new Bounds();

    public bool Touching;

    public PlayerController Player { get { return _player; } }
    public PlanetController ActivePlanet {  get { return _activePlanet; } }
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
        _player.gameObject.transform.position = new Vector3(2, 0, 0);
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
        while (next != null && count <50)
        {
			Logger.Log("Branch " + count + " from Planet " + next.ID);
            Branch(next);
            next = GetNextBranch();
            count++;
        }
        return true;
    }

    private PlanetController GetNextBranch()
    {
		for(int i=0; i<_planets.Count; i++)
		{
			if(!_planets[i].full)
			{
				return _planets[i];
			}
		}
        return null;
    }

    private void Branch(PlanetController pc)
    {
		float newPlanetRadius = UnityEngine.Random.Range(2.0f, 3.5f);
		Logger.Log("New Planet Radius is " + newPlanetRadius);

		// Find What other planets are in range
		List<PlanetController> closePlanets = new List<PlanetController>();
		bool anyPlanetToLeft = false;
		for(int i=0; i<_planets.Count; i++)
		{
			if(_planets[i] == pc){continue;} // Planet is this planet.
			if(_planets[i].Center.y + _planets[i].GravityRadius < pc.Center.y - pc.GravityRadius){continue;}// Planet is too far below this one.

            if(_planets[i].Center.x < pc.Center.x)
            {
                // Planet is to the left, include it.
                closePlanets.Add(_planets[i]);
                anyPlanetToLeft = true;
            }
            else if(_planets[i].Center.y > pc.Center.y)
            {
                //Planet is right and above
                closePlanets.Add(_planets[i]);
                anyPlanetToLeft = true;
            }
		}

		// Determine which Close planet to use for placement.
		PlanetController closest = null;

		for(int i=0; i<closePlanets.Count; i++)
		{
			PlanetController temp = closePlanets[i];
			if(temp.Center.x > pc.Center.x && temp.Center.y <= pc.Center.y){continue;}

			if(temp.Center.x < pc.Center.x)
			{
				// Left of source, Pick largest y
				if(closest == null || temp.Center.y > closest.Center.y)
				{
					closest = temp;
				}
			}
			else
			{
				//Right of source, pick largest x
				if(closest == null || temp.Center.x > closest.Center.x)
				{
					closest = temp;
				}
			}
		}

		if(closest != null && anyPlanetToLeft)
		{

			// Calculate desired position of new planet branch.
			float ON_Length = pc.GravityRadius + newPlanetRadius;
			float BN_Length = closest.GravityRadius + newPlanetRadius;
			Circle A = new Circle(pc.Center, ON_Length);
			Circle B = new Circle(closest.Center, BN_Length);
			Vector2 intersect1, intersect2;
			int intersections = Utilities.GetCircleIntersections(A, B, out intersect1, out intersect2);
			Vector3 newPosition = Vector3.zero;
			if(intersections == 0 || intersections == 1 )
			{
				Logger.Log("Error Placing Planet. Planet " + pc.ID + " is Full.");
				pc.full = true;
				return;
			}
			else 
			{
                // Need to make sure its getting the intersection to the right.
                Vector3 baseLine = pc.gameObject.transform.InverseTransformPoint(closest.Center);
       
				Vector3 angle1 = pc.gameObject.transform.InverseTransformPoint(intersect1);
				Vector3 angle2 = pc.gameObject.transform.InverseTransformPoint(intersect2);
                
                float angle1Direction = Utilities.AngleDirection3(baseLine, angle1, -Vector3.forward);
				float angle2Direction = Utilities.AngleDirection3(baseLine, angle2, -Vector3.forward);
                Logger.Log("BaseLineVector " + baseLine + "angle1: " + angle1 + "angle2: " + angle2 + "angle1Dir: " + angle1Direction + "angle2Dir: " + angle2Direction);
                if (angle1Direction > 0)
				{
                    Logger.Log("Intersect 1 is to the right.");
                    newPosition = intersect1;
                }
				else if(angle2Direction > 0)
				{
                    Logger.Log("Intersect 2 is to the right.");
					newPosition = intersect2;
				}
				else
				{
					Logger.Log("Error Placing Planet. No Valid Angle Direction. Planet " + pc.ID + " is Full.");
					pc.full = true;
					return;
				}

			}

			Circle newPlanetCircle = new Circle(newPosition, newPlanetRadius);
			Logger.Log("Closest Branch Planet is: " + closest.ID);
			Logger.Log("Attempting to branch new planet at " + newPosition);


			bool canPlace = CanPlace(newPlanetCircle);
			if(canPlace)
			{
				// Place new planet relative to Closest.
				Logger.Log("New planet placement Successful.");
				PlacePlanet(0.1f, newPlanetRadius, newPosition);
                return;
			}
			else
			{
                if (!CreatePlanetOnLeftEdge(pc, newPlanetRadius))
                {
                    Logger.Log("There is no space against the wall. Planet " + pc.ID + " is full.");
                    pc.full = true;
                }
            }
		}
		else // No planets are in range
		{
			Logger.Log("There are no valid Branching Planets from Planet " + pc.ID);
            if (!CreatePlanetOnLeftEdge(pc, newPlanetRadius))
            {
                Logger.Log("There is no space against the wall. Planet " + pc.ID + " is full.");
                pc.full = true;
            }
		}
    }

    private bool CreatePlanetOnLeftEdge(PlanetController pc, float newPlanetRadius)
    {
        float spaceBetweenPlanetAndBounds = pc.Center.x - pc.GravityRadius - _bounds.min.x;
        Vector3 newPos;
        if (spaceBetweenPlanetAndBounds >= newPlanetRadius * 2)
        {
            //Try Placing new planet directly to the left.
            newPos = pc.Center - new Vector3(pc.GravityRadius + newPlanetRadius, 0, 0);
            bool canPlaceToLeft = CanPlace(new Circle(newPos, newPlanetRadius));
            if (canPlaceToLeft)
            {
                Logger.Log("Placing ne planet directly to the left of planet " + pc.ID);
                PlacePlanet(0.5f, newPlanetRadius, newPos);
            }
            else
            {
                return false;
            }
        }
        else
        {
            // Place Against the Left wall above this planet
            float newXpos = _bounds.min.x + newPlanetRadius;
            float x = pc.Center.x - newXpos;
            float h = pc.GravityRadius + newPlanetRadius;
            float newYpos = (Mathf.Sqrt((h * h) - (x * x)) + pc.Center.y);
            newPos = new Vector2(newXpos, newYpos);
            bool canPlace = CanPlace(new Circle(newPos, newPlanetRadius));
            if (canPlace)
            {
                Logger.Log("Placing new planet against the left border at " + newXpos + ", " + newYpos);
                PlacePlanet(0.5f, newPlanetRadius, new Vector3(newXpos, newYpos, 0));
            }
            else
            {
                Logger.Log("There is no space against the wall. Planet " + pc.ID + " is full.");
                return false;
            }
        }
        return true;
    }

	private void CreateRefBall(Vector3 pos, string name, float rad = .1f)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.position = pos;
		go.transform.localScale = new Vector3(rad*2.0f, rad*2.0f, rad*2.0f);
		go.name = name;
	}

	private bool CanPlace(Circle newPlanetCircle)
	{
		bool canPlace = true;
		// checks if this circle can be placed on the board without overlapping any borders or any other planets.
		if(Utilities.CrossesBorder(newPlanetCircle, _bounds.max.x))
		{
			Logger.Log("New planet overlaps Right wall.");
			canPlace = false;
		}
		if(Utilities.CrossesBorder(newPlanetCircle, _bounds.min.x))
		{
			Logger.Log("New planet overlaps Left wall.");
			canPlace = false;
		}
		// If it overlaps any other circle, its not valid.
		for(int i=0; i<_planets.Count; i++)
		{
			Circle existingPlanet = new Circle(_planets[i].Center, _planets[i].GravityRadius);
			if(Utilities.CirclesOverlap(newPlanetCircle, existingPlanet))
			{
				Logger.Log("New planet overlaps the existing planet: " + _planets[i].ID);
				canPlace = false;
				break;
			}
		}
		return canPlace;
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
		pc5.ID = _planets.Count;
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
        _activePlanet = planet;

        changeUIText(planet.Center.y.ToString("0"));
    }

    public void changeUIText(string s)
    {
        _uicontrol.setMessageText(s);
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