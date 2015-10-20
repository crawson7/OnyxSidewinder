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
        if (Logger.Initialize())
        { Logger.Log("Logger Initialization Successful."); }
        else { Debug.LogError("Logger Initialization Failed"); return false; }

        if (InputManager.Instance.Initialize())
        { Logger.Log("Input Manager Initialization Successful."); }
        else { Debug.LogError("Input Manager Initialization Failed"); return false; }

        return true;
    }

    private bool LoadData()
    {
        return true;
    }

    private bool LateInitialize()
    {
        return true;    
    }

    private void StartGame()
    {
        Game.Instance.Initialize();
        GameInitialized = true;
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
