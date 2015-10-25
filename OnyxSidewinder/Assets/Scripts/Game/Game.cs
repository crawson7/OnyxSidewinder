using UnityEngine;
using System.Collections.Generic;

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

    public GameObject GameObj;
    public GameObject DialogsObj;
    public bool Touching;

    public PlayerController Player { get { return _player; } }

    public bool Initialize()
    {
        GameObj = GameObject.Find("Game");
        DialogsObj = GameObject.Find("Dialogs");
        if(GameObj == null || DialogsObj == null) { Logger.Log("Could not find parent objects"); return false; }

        if (!LoadPlayer()) { return false; }
        if (!LoadPlanets()) { return false; }
        Logger.Log("Game System Initialized.");
        _active = true;
        return true;
    }

    public void Update()
    {
        if(!_active) { return; }

        _checkInterval += Time.deltaTime;
        if (_checkInterval >= _checkFrequency)
        {
            if (!_player.Orbiting)
            { CheckShipAttach(); }
        }
    }

    private void CheckShipAttach()
    {
        for (int i = 0; i < _planets.Count; i++)
        {
            if (!_planets[i].Active) { continue; }
            if (_planets[i].TestCollision())
            {
                return;
            }
        }
    }


    public bool LoadPlayer()
    {
        GameObject prefab = Resources.Load("Game/Player") as GameObject;
        GameObject player = GameObject.Instantiate<GameObject>(prefab);
        _player = player.GetComponent<PlayerController>();
        _player.gameObject.transform.position = Vector3.zero;
        _player.gameObject.transform.SetParent(GameObj.transform, false);
        if (!_player.Initialize()) { Logger.Log("Player did not initialize propperly"); return false; }
        return true;
    }

    public bool LoadPlanets()
    {
        // This will all eventually be either procedural or from data.
        GameObject prefab = Resources.Load("Game/Planet") as GameObject;

        GameObject planet1 = GameObject.Instantiate<GameObject>(prefab);
        if(planet1 == null) { return false; }
        PlanetController pc = planet1.GetComponent<PlanetController>();
        pc.Initialize(0.75f, 3.0f);
        _planets.Add(pc);
        pc = planet1.GetComponent<PlanetController>();
        pc.gameObject.transform.position = new Vector3(4, 5, 0);
        pc.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet2 = GameObject.Instantiate<GameObject>(prefab);
        if (planet2 == null) { return false; }
        PlanetController pc2 = planet2.GetComponent<PlanetController>();
        pc2.Initialize(0.5f, 3.0f);
        _planets.Add(pc2);
        pc2 = planet2.GetComponent<PlanetController>();
        pc2.gameObject.transform.position = new Vector3(-4, 5, 0);
        pc2.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet3 = GameObject.Instantiate<GameObject>(prefab);
        if (planet3 == null) { return false; }
        PlanetController pc3 = planet3.GetComponent<PlanetController>();
        pc3.Initialize(1f, 3.0f);
        _planets.Add(pc3);
        pc3 = planet3.GetComponent<PlanetController>();
        pc3.gameObject.transform.position = new Vector3(4, -5, 0);
        pc3.gameObject.transform.SetParent(GameObj.transform, false);

        GameObject planet4 = GameObject.Instantiate<GameObject>(prefab);
        if (planet4 == null) { return false; }
        PlanetController pc4 = planet4.GetComponent<PlanetController>();
        pc4.Initialize(0.25f, 3.0f);
        _planets.Add(pc4);
        pc4 = planet4.GetComponent<PlanetController>();
        pc4.gameObject.transform.position = new Vector3(-4, -5, 0);
        pc4.gameObject.transform.SetParent(GameObj.transform, false);
        return true;
    }

    public void Start()
    {
        _player.SetSpeed(8.0f);
    }

    public void HandleTouch(Vector2 pos)
    {
        _player.Release();
    }

    public void HandleRelease(Vector2 pos)
    {
        // Do nothing for now.
    }

    public void HandlePlanetCollide(PlanetController planet)
    {
        _player.Kill();
        End(false);
    }

    public void HandleGravityCollide(PlanetController planet)
    {
        for(int i=0; i<_planets.Count; i++)
        {
            _planets[i].Active = true;
        }
        _player.Orbit(planet);
    }


    public void End(bool win)
    {
        _active = false;
        Logger.Log("GAME OVER");
    }

}
