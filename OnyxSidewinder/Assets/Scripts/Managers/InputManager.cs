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

    [HideInInspector] public Vector2 LastTouch;
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
	
	public void Update ()
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
			if(CtlState){Logger.LogLevel = 0;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(CtlState){Logger.LogLevel = 1;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(CtlState){Logger.LogLevel = 2;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if(CtlState){Logger.LogLevel = 3;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			if(CtlState){Logger.LogLevel = 4;}
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			if(CtlState){Logger.LogLevel = 5;}
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
        }
    }
    #endregion

    #region Platform Agnostic Inputs
    private void Touch(Vector2 pos)
    {
        Logger.Log("Touch at: (" + pos.x + ", " + pos.y + ")", 2);
        LastTouch = pos;
        Touching = true;
        Game.Instance.HandleTouch(pos);
    }

    private void Release(Vector2 pos)
    {
        Logger.Log("Release at: (" + pos.x + ", " + pos.y + ")", 2);
        Touching = false;
        Game.Instance.HandleRelease(pos);
    }

    private void Drag(Vector2 delta, Vector2 total)
    {
		Logger.Log("Dragging " + delta.x + ", " + delta.y + " - Total Drag: " + total.x + ", " + total.y, 0);
    }
    #endregion 
}
