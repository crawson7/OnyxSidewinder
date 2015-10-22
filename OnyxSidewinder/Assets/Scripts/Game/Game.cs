using UnityEngine;
using System.Collections;

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
        Logger.Log("Game System Initialized.");
        return true;
    }

    public void Update()
    {
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

    public void Start()
    {
    }

    public void HandleTouch(Vector2 pos)
    {
        Vector3 forward = _player.Forward;
        Vector3 force = forward * 2.5f;
        _player.Body.AddForce(force, ForceMode2D.Impulse);
        _player.Body.angularVelocity = 0;
    }

    public void HandleRelease(Vector2 pos)
    {
        float magnitude = _player.Body.velocity.magnitude;
        float circumfrence = 2.0f * Mathf.PI * _player.Radius;
        float torque = (7.5f / circumfrence) * magnitude; // 7.5 is the time for 1 full rotation with 1 torque.
        _player.Body.AddTorque(torque, ForceMode2D.Impulse);
        _player.Body.velocity = Vector2.zero;
    }
}
