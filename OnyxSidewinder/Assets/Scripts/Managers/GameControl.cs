using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

    #region Singleton Access
    private static GameControl _instance;

    public static GameControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Main").GetComponent<GameControl>();
            }
            return _instance;
        }
    }
    #endregion

    private bool GameInitialized = false;
	public int LogLevel;

    // Use this for initialization
    void Start ()
    {
        Debug.Log("Entering Game Control");

        if (!EarlyInitialize()) { StartUpError("Early Initialization Failed."); return; }
        if(!LoadData()) { StartUpError("Data Load Failed."); return; }
        if(!LateInitialize()) { StartUpError("Late Initialization Failed."); return; }

        StartGame();
	}
	
	// Update is called once per frame
	void Update () {
	    if(GameInitialized)
        {
            InputManager.Instance.Update();
            Game.Instance.Update();
        }
	}

    void LateUpdate()
    {

    }

    void FixedUpdate()
    {

    }

    private bool EarlyInitialize()
    {
        if (Logger.Initialize(LogLevel))
        { Logger.Log("Logger Initialization Successful.", 1); }
        else { Debug.LogError("Logger Initialization Failed"); return false; }

        if (InputManager.Instance.Initialize())
        { Logger.Log("Input Manager Initialization Successful.", 1); }
        else { Logger.Log("Input Manager Initialization Failed", 5); return false; }

        return true;
    }

    private bool LoadData()
    {
        return true;
    }

    private bool LateInitialize()
    {
        if (CameraManager.Instance.Initialize())
        { Logger.Log("Camera Manager Initialization Successful.", 1); }
        else { Logger.Log("Camera Manager Initialization Failed", 5); return false; }
        return true;    
    }

    private void StartGame()
    {
        if(!Game.Instance.Initialize())
        {
            Logger.Log("Game System could not initialize", 5);
            return;
        }

        GameInitialized = true;
        Game.Instance.Start();
    }

    private void StartUpError(string s)
    {
        if (Logger.Active)
        { Logger.Log(s, 3); }
        else
        {
            Debug.LogError(s);
        }
    }
}
