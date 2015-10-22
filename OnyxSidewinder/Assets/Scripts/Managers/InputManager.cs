using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    #region Singleton Access
    private static InputManager _instance;

    public static InputManager Instance
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
      
        if (Input.GetMouseButtonDown(0) && !Touching)
        {
            Touch(pos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // No effect
        }
        if(Input.GetMouseButtonUp(0) && Touching)
        {
            Release(pos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // No effect
        }
        LastTouchPos = pos;
    }

    private void CheckKeys()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            AltState = true;
        }
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            AltState = false;
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            Logger.Log("You pressed the letter 'A'");
        }
    }
    #endregion

    #region Touch Specific Inputs
    private void CheckTouchInputs()
    {
    }
    #endregion

    #region Platform Agnostic Inputs
    private void Touch(Vector2 pos)
    {
        Logger.Log("Touch at: (" + pos.x + ", " + pos.y + ")");
        LastTouch = pos;
        Touching = true;
        Game.Instance.HandleTouch(pos);
    }

    private void Release(Vector2 pos)
    {
        Logger.Log("Release at: (" + pos.x + ", " + pos.y + ")");
        Touching = false;
        Game.Instance.HandleRelease(pos);
    }

    private void Drag(Vector2 delta, Vector2 total)
    {
        Logger.Log("Dragging " + delta.x + ", " + delta.y + " - Total Drag: " + total.x + ", " + total.y);
    }
    #endregion 
}
