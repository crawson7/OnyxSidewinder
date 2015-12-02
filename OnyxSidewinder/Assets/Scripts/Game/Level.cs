using UnityEngine;
using System.Collections;

public enum LevelState
{
    Unready,
    Playing,
    Paused,
    Stopped

}

public class Level
{
    // State Machine
    private PlanetManager _planets;
    private LevelData _data;
    private Bounds _bounds;
    private Vector3 _startPosition;
    private LevelState _state;
    private OnyxUIController _uicontrol;
    private bool _debugCameraEnabled = false;
    private DebugPrefabController _debugCameraTargetController;
    private DebugPrefabController _debugCameraPositionController;

    // World Settings
    private float _gravity;
    private float _rotationAcceleration;
    private float _equilibriumSpeed;
    private float _orbitAccelerationTime;

    public Bounds Bounds { get { return _bounds; } }
    public PlanetManager Planets { get { return _planets; } }
    public LevelState State {get{return _state;} set{_state = value;}}
	public string Name {get{return _data.SceneName; }}

    public bool DebugCameraEnabled { get { return _debugCameraEnabled; } }
    public DebugPrefabController CameraTarget { get { return _debugCameraTargetController; } }
    public DebugPrefabController CameraPosition { get { return _debugCameraPositionController; } }

    public Level()
    {
        State = LevelState.Unready;
    }

    public bool Load(LevelData data)
    {
        // Load the scene specified for this level.
        _data = data;
        if (!SceneManager.Instance.LoadScene(data.SceneName, () => { Initialize(); }))
        { return false; }

		return true;
	}

    public void Resart()
    {
        Planets.Restart();
        State = LevelState.Stopped;
    }

    public void SetLevelSettings(LevelSettingsEditor settings)
    {
        _planets = new PlanetManager();
        _planets.Initialize(settings);

        _bounds = new Bounds(settings.Bounds.position, settings.Bounds.size);
        _startPosition = settings.StartPosition;
        _gravity = settings.Gravity;
        _rotationAcceleration = settings.RotationAcceleration;
        _equilibriumSpeed = settings.EquilibriumSpeed;
        _orbitAccelerationTime = settings.OrbitAccelerationTime;
    }

    private void BuildLevel()
    {
        Logger.Log("Building Level " + _data.ID, 1);
        _planets.BuildNewLevel();
    }

    public void Initialize()
    {
        if (!VerifyData()) { Logger.Log("Level Data has not been set. Level Initialize Failed.", 4); return; }
        if (!InitializeManagers()) { Logger.Log("Level Managers Failed to initialize", 4); return; }

        BuildLevel();
        Game.Instance.LoadPlayer(_startPosition);
        LoadUI();
        //LoadDebugTools();
        State = LevelState.Stopped;
    }

    private bool VerifyData()
    {
        if(_data !=null && _bounds != null)
        {
            return true;
        }
        return false;
    }

    private bool InitializeManagers()
    {
        // TODO: State Machine
        // TODO: Score Keeper
        // TODO: Objectives Manager
        return true;
    }

    public void Start()
    {
        State = LevelState.Playing;
    }

    public bool LoadUI()
    {
        GameObject prefab = Resources.Load("UI/GameHUD") as GameObject;
        GameObject GameHUD = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        GameHUD.transform.SetParent(SceneManager.Instance.Dialogs.transform, false);
        _uicontrol = GameHUD.GetComponent<OnyxUIController>();
        if (!_uicontrol.Initialize()) { Logger.Log("UI did not initialize properly", 5); return false; }

        return true;
    }

    public bool LoadDebugTools()
    {
        if (Logger.LogLevel > 0) { _debugCameraEnabled = true; }

        if (_debugCameraEnabled)
        {
            GameObject prefab = Resources.Load("Debug/DebugPrefab") as GameObject;
            GameObject debugCameraTarget = GameObject.Instantiate<GameObject>(prefab);
            _debugCameraTargetController = debugCameraTarget.GetComponent<DebugPrefabController>();
            _debugCameraTargetController.gameObject.transform.position = Vector3.zero;
            _debugCameraTargetController.gameObject.transform.SetParent(Game.Instance.GameObj.transform, false); //this might be a "Debug" meta-class in the future.
            _debugCameraTargetController.setSprite("CircleCross");
            _debugCameraTargetController.setColor(Color.red);

            GameObject prefab2 = Resources.Load("Debug/DebugPrefab") as GameObject;
            GameObject debugCameraPosition = GameObject.Instantiate<GameObject>(prefab2);
            _debugCameraPositionController = debugCameraPosition.GetComponent<DebugPrefabController>();
            _debugCameraPositionController.gameObject.transform.position = Vector3.zero;
            _debugCameraPositionController.gameObject.transform.SetParent(Game.Instance.GameObj.transform, false); //this might be a "Debug" meta-class in the future.
            _debugCameraPositionController.setSprite("CircleCross");
            _debugCameraPositionController.setColor(Color.green);
        }
        return true;
    }

    public void updateUIJumps(string s)
    {
        _uicontrol.setJumpsText("Jumps: " + s);
    }

    public void updateUIDistance(string s)
    {
        _uicontrol.setDistanceText("Distance: " + s + "/50");
    }
}
