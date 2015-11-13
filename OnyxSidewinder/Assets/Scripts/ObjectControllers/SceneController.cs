using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public string Name;
    public LevelSettingsEditor LevelSettings;

	void Start ()
    {
        Logger.Log("Starting New Scene.");
        SceneManager.Instance.RegisterScene(this);
	}

    public bool Initialize()
    {
        // Load all of the level settings and Remove Editor Objects.
        Game.Instance.CurrentLevel.SetLevelData(LevelSettings);

        // TODO: Destroy the Settings Editor and all the Editor Objects.
        return true;
    }

    public void Terminate()
    {
        // clean up any loose ends befor being deleted, but do not delete the scene object.
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
