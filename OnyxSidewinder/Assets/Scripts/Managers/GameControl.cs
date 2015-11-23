using UnityEngine;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    #region Singleton Access
    private static GameControl _instance;

    public static GameControl Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject main = GameObject.Find("Main");
                if(main == null) { return null; }
                _instance = main.GetComponent<GameControl>();
            }
            return _instance;
        }
    }
    #endregion

    private bool GameInitialized = false;
	public int LogLevel;
    public string StartScene;
    private List<StateMachine> _stateMachines = new List<StateMachine>();

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

        if (SceneManager.Instance.Initialize())
        { Logger.Log("Scene Manager Initialization Successful.", 1); }
        else { Logger.Log("Scene Manager Initialization Failed", 5); return false; }

		DataManager.Initialize();
        return true;
    }

    private bool LoadData()
    {
        SceneManager.Instance.LoadSceneData();
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
        
        if(!Game.Instance.LoadSaveData())
        {
            Logger.Log("Game Could not load Save Data.", 5);
            return;
        }
        
        GameInitialized = true;
        Game.Instance.Start(StartScene);
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

    public void RegisterStateMachine(StateMachine sm)
    {
        // First make sure that this State Machine doe not already exist.
        for(int i=0; i<_stateMachines.Count; i++)
        {
            if(_stateMachines[i] == sm) { return; }
        }
        _stateMachines.Add(sm);
    }

    public void RemoveStateMachine(StateMachine sm)
    {
        for (int i = 0; i < _stateMachines.Count; i++)
        {
            if (_stateMachines[i] == sm) { _stateMachines.RemoveAt(i); return; }
        }
    }




}
