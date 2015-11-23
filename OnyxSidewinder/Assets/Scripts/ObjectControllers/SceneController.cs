using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
    public string Name;
    public LevelSettingsEditor LevelSettings;
    public List<string> SceneDialogs = new List<string>();

	void Start ()
    {
        Logger.Log("Starting New Scene.");
        if(GameControl.Instance == null)
        {
            // TODO: If on its own, load the game first.
            Logger.Log("This scene " + Name + " is running on its own, outside the context of the game.", 3);

        }
        else
        {
            SceneManager.Instance.RegisterScene(this);
        }
	}

    public bool Initialize()
    {
        // TODO: this is going to need to know what kind of scene it is, or at least
        // be able to handle not having all the level data, because scenes may not nessesarily be levels.
        
        // Load all of the level settings and Remove Editor Objects.
        if(LevelSettings == null)
        {
            Logger.Log("Level Settings do not exist for this Level.", 3);
            InitializeAsMenu();
        }
        else
        {
            InitializeAsLevel();
        }
        
        return true;
    }

    private void InitializeAsLevel()
    {
        // TODO: Destroy the Settings Editor and all the Editor Objects.
        Game.Instance.CurrentLevel.SetLevelSettings(LevelSettings);
    }

    private void InitializeAsMenu()
    {
        for(int i=0; i<SceneDialogs.Count; i++)
        {
            GameObject go = Resources.Load("UI/" + SceneDialogs[i]) as GameObject;
            GameObject dialog = (GameObject) Object.Instantiate(go, Vector3.zero, Quaternion.identity);
            dialog.transform.SetParent(SceneManager.Instance.Dialogs.transform, false);
            dialog.transform.localPosition = Vector3.zero;
        }
    }

    public void Terminate()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach ( Transform t in SceneManager.Instance.Dialogs.transform)
        {
            toRemove.Add(t.gameObject);
        }
        for(int i=0; i<toRemove.Count; i++)
        {
            Object.Destroy(toRemove[i]);
        }
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
