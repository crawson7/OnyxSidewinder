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

    public bool Initialize()
    {
        Logger.Log("Game System Initialized.");
        return true;
    }

    public void Update()
    {

    }
}
