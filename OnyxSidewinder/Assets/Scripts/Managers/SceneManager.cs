using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SceneManager : MonoBehaviour
{
	#region Singleton Access
	private static SceneManager _instance;

	public static SceneManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.Find("Main").GetComponent<SceneManager>();
			}
			return _instance;
		}
	}
	#endregion

	public GameObject Cameras, Dialogs, GameParent;
	private Queue<LoadRequest> _loadRequests = new Queue<LoadRequest>();
	private LoadRequest _activeLoad;
    private Dictionary<string, SceneController> _activeScenes = new Dictionary<string, SceneController>();
    private List<SceneController> _registeredScenes = new List<SceneController>();
    private List<SceneData> _scenes = new List<SceneData>();

	public bool Initialize()
	{
		if(!VerifyMainSceneStructure()){return false;}
        ClearParentScenes();
		return true;
	}

	public void Terminate()
	{
		
	}

	private void Update()
	{
        // Check the progress of any active load request.
		if(_activeLoad != null)
		{
			if(_activeLoad.Async.isDone)
			{
				Logger.Log("Asyncronous Load Opperation Done.", 1);
                ActivateScene(_activeLoad);
				_activeLoad.RunCallback();
				_activeLoad = null;
			}
			else
			{
				Logger.Log("Progress " + _activeLoad.Async.progress, 0);
			}
		}
        // If there is not an active load request. Start the next request in the queue
		else if(_loadRequests.Count>0)
		{
            Logger.Log("Ready to Load Scene. Request List Length = " + _loadRequests.Count, 1);
            LoadRequest load = _loadRequests.Dequeue();
            StartCoroutine(LoadSceneAsync(load));
		}
	}


	public bool VerifyMainSceneStructure()
	{
		//Verify Cameras
		if(Cameras == null)
		{
			GameObject cameras = Resources.Load("Platform/Cameras") as GameObject;
			GameObject camsObj = (GameObject) GameObject.Instantiate(cameras, Vector3.zero, Quaternion.identity);
			if(camsObj == null){return false;}
			camsObj.name = "Cameras";
			Cameras = camsObj;
		}

		// Verify Dialogs
		if(Dialogs == null)
		{
			GameObject dialogs = Resources.Load("Platform/Dialogs") as GameObject;
			GameObject uiObj = (GameObject) GameObject.Instantiate(dialogs, Vector3.zero, Quaternion.identity);
			if(uiObj == null){return false;}
			uiObj.name = "Cameras";
			Dialogs = uiObj;
		}

		// Verify Game Container
		if(Cameras == null)
		{
			GameObject game = Resources.Load("Platform/Game") as GameObject;
			GameObject gameObj = (GameObject) GameObject.Instantiate(game, Vector3.zero, Quaternion.identity);
			if(gameObj == null){return false;}
			gameObj.name = "Cameras";
			GameParent = gameObj;
		}
		return true;
	}

    public bool LoadSceneData()
    {
        SceneData s1 = new SceneData();
        s1.Name = "StartScene";
        s1.Id = 0;
        s1.Type = SceneType.Main;

        SceneData s2 = new SceneData();
        s2.Name = "Test";
        s2.Id = 1;
        s2.Type = SceneType.Level;

        _scenes.Add(s1);
        _scenes.Add(s2);
        return true;
    }

    private void ActivateScene(LoadRequest load)
    {
        // By the time the scene is activated, it should have been finished loading and the new scene controller
        // should have already registered itself with the SceneManager.
        // Find the registered SceneController, Initialize it and add it to the active scenes Dict.
        for(int i=0; i<_registeredScenes.Count; i++)
        {
            if(load.SceneName == _registeredScenes[i].Name)
            {
                if (!_registeredScenes[i].Initialize())
                {
                    Logger.Log("New Scene: " + _registeredScenes[i].Name + " Could not be initialized.", 4);
                    return;
                }

                _activeScenes.Add(_registeredScenes[i].Name, _registeredScenes[i]);
                _registeredScenes[i].gameObject.transform.SetParent(GameParent.transform, false);
                _registeredScenes.RemoveAt(i);
                return;
            }
        }
        Logger.Log("New Loaded Scene: " + load.SceneName + " is not yet registered.\rIt could be that the scene " + load.SceneName + " does not have a scene controller attached.", 3);
    } 

    public void RegisterScene(SceneController scene)
    {
        _registeredScenes.Add(scene);
    }

	public void ClearParentScenes()
	{
        UnloadAllScenes();
        _activeScenes.Clear();
        foreach(Transform t in GameParent.transform)
        {
            UnityEngine.Object.Destroy(t.gameObject);
        }
        foreach (Transform t in Dialogs.transform)
        {
            if(t.name != "EventSystem") { UnityEngine.Object.Destroy(t.gameObject); }
        }
	}

	public void LoadScene(string name, Action onComplete=null)
	{
        if (!IsValidScene(name)) { Logger.Log("Scene " + name + " is not a valid Scene. Load Failed.", 3);  return; }
        
        _loadRequests.Enqueue(new LoadRequest(name, onComplete));
    }

	public void UnloadScene(string name, Action onComplete=null)
	{
        if (!IsActiveScene(name)) { Logger.Log("Scene " + name + " is not a valid Scene. Unload Failed.", 3); return; }

        SceneController sc = _activeScenes[name];
        sc.Terminate();
        _activeScenes.Remove(name);
        UnityEngine.Object.Destroy(sc.gameObject);
        Logger.Log("Scene " + name + " successfully unloaded.", 1);
    }

	public void UnloadAllScenes(Action onComplete=null)
	{
		foreach(string s in _activeScenes.Keys)
        {
            UnloadScene(s);
        }
	}

	public IEnumerator LoadSceneAsync(LoadRequest load)
	{
        _activeLoad = load;
        string name = _activeLoad.SceneName;
		Logger.Log("Start Loading Scene " + name + " at " + Time.time, 2);
        _activeLoad.Async = Application.LoadLevelAdditiveAsync(name);
		yield return _activeLoad.Async;
		Logger.Log("Finished Loading Scene: " + name + " at " + Time.time, 2);
        // Opportunity here to run code on complete prior to sceneController start();
        // TODO Send Scene Load Complete Signal.
	}

    private bool IsValidScene(string name)
    {
        // Checks that this is a valid scene that can be loaded into the StartScene.
        for(int i=0; i<_scenes.Count;i++)
        {
            if(_scenes[i].Name == name)
            {
                if(_scenes[i].Type== SceneType.Level || _scenes[i].Type == SceneType.UI)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsActiveScene(string name)
    {
        return (_activeScenes.ContainsKey(name));
    }

	public SceneController GetScene(string name)
	{
        if (!IsActiveScene(name)) { return null; }
        return _activeScenes[name];
	}

    public bool MoveToScene(GameObject go, string scene, bool worldPositionStays)
    {
        if (!IsActiveScene(scene)){ return false; }

        go.transform.SetParent(_activeScenes[name].gameObject.transform, worldPositionStays);
        return true;
    }
}

public class LoadRequest
{
	public Action OnComplete;
    public string SceneName;
	public AsyncOperation Async;

	public LoadRequest(string name, Action callback)
	{
        SceneName = name;
		OnComplete = callback;
	}

	public void RunCallback()
	{
		if(OnComplete !=null)
		{
			OnComplete();
		}
	}
}
