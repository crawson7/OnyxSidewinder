using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    #region Singleton Access
    private static InputManager _instance;

    public static InputManager Instance // Is there a reason that this isn't more descriptive? -jake
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.Find("Main").GetComponent<InputManager>();
            }
            return _instance;
        }
    }
    #endregion

    private bool _editor = false;
    private bool _ios = false;
    private bool _android = false;

    [HideInInspector] public Vector2 LastTouchPos;
    [HideInInspector] public bool Touching;
    [HideInInspector] public bool AltState;
	[HideInInspector] public bool CtlState;

    public bool Initialize()
    {
#if UNITY_EDITOR
        _editor = true;
#endif

#if UNITY_IOS
        _ios = true;
#endif

#if UNITY_ANDROID
        _android = true;
#endif
        return true;
    }

    public void Terminate()
    {
    }
	
	private void Update ()
    {
        if(_editor)
        {
            CheckPCInputs();
        }
        else if(_ios || _android)
        {
            CheckTouchInputs();
        }   
    }

    #region PC Specific Inputs
    private void CheckPCInputs()
    {
        CheckMouse();
        CheckKeys();
    }

    private void CheckMouse()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 delta = pos - LastTouchPos;
      
        // Left button controls
        if (Input.GetMouseButtonDown(0) && !Touching)
        {
            Touch(pos);
        }
        if(Input.GetMouseButtonUp(0) && Touching)
        {
            Release(pos);
        }
        if(Input.GetMouseButton(0) && Touching)
        {
            Drag(delta, pos);
        }

        // Right button controls
        if (Input.GetMouseButtonDown(1))
        {
            // No effect
        }
        if (Input.GetMouseButtonUp(1)) 
        {
            // No effect
        }
        LastTouchPos = pos;
    }

    private void CheckKeys()
    {
		AltState = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
		CtlState = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

        if(Input.GetKeyDown(KeyCode.A))
        {
            CameraManager.Instance.GameCam.gameObject.transform.position =
                new Vector3(Game.Instance.Player.Position.x, Game.Instance.Player.Position.y, -10) ;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraManager.Instance.GameCam.AddBehavior("PlayerCloseFollow", 0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CameraManager.Instance.GameCam.RemoveBehavior("PlayerCloseFollow");
        }
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			if(AltState){Logger.LogLevel = 0;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(AltState){Logger.LogLevel = 1;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(AltState){Logger.LogLevel = 2;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if(AltState){Logger.LogLevel = 3;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			if(AltState){Logger.LogLevel = 4;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			if(AltState){Logger.LogLevel = 5;}
		}
		if(Input.GetKeyDown(KeyCode.L))
		{
			if(AltState)
			{
				Logger.Log("Loading Scene Test"); 
				SceneManager.Instance.LoadScene("Test", ()=> { Logger.Log("Scene Load On Complete Callback", 1); });
			}
		}
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (AltState)
            {
                Logger.Log("Unloading Scene Test");
                SceneManager.Instance.UnloadScene("Test");
            }
        }
		if(Input.GetKeyDown(KeyCode.D))
		{
			if (AltState)
			{
				Logger.Log("Test Loading Level Data");
				LevelData ld;
				DataManager.Load<LevelData>(out ld);
				Logger.Log("Loaded Data is " + ld.ID, 1);
			}
		}
		if(Input.GetKeyDown(KeyCode.S))
		{
			if (AltState)
			{
				Logger.Log("Test Saving Level Data");
				LevelData ld = new LevelData();
				ld.ID = 10;
				DataManager.Save<LevelData>(ld);
			}
		}
    }
    #endregion

    #region Touch Specific Inputs
    private void CheckTouchInputs()
    {
        if(Input.touchCount>0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch(Input.GetTouch(0).position);
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Release(Input.GetTouch(0).position);
            }
            if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Drag(Input.GetTouch(0).deltaPosition, Input.GetTouch(0).position);
            }
        }
    }
    #endregion

    #region Platform Agnostic Inputs
    private void Touch(Vector2 pos)
    {
        Logger.Log("Touch at: (" + pos.x + ", " + pos.y + ")", 2);
        LastTouchPos = pos;
        Touching = true;
        Game.Instance.HandleTouch(pos);
    }

    private void Release(Vector2 pos)
    {
        Logger.Log("Release at: (" + pos.x + ", " + pos.y + ")", 2);
        Touching = false;
        Game.Instance.HandleRelease(pos);
    }

    private void Drag(Vector2 delta, Vector2 pos)
    {
        LastTouchPos = pos;
        Game.Instance.HandleDrag(delta, pos);
		Logger.Log("Dragging " + delta.x + ", " + delta.y + " - Total Drag: " + pos.x + ", " + pos.y, 0);
    }
    #endregion 
}
