using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class CameraController : MonoBehaviour
{
    private Camera _cam;
    private List<BaseCameraBehavior> _activeBehaviors;
    private Dictionary<string, BaseCameraBehavior> _allBehaviors;
    private bool _initialized = false;

    public Camera Cam {get{return _cam;}}
    public Transform Obj {get{return gameObject.transform;}}

    public bool Initialize()
    {
        _cam = gameObject.GetComponent<Camera>();
        if(Cam == null) { return false; }
        _activeBehaviors = new List<BaseCameraBehavior>();
        _allBehaviors = new Dictionary<string, BaseCameraBehavior>();
        LoadBehaviors();
        _initialized = true;
        return true;
    }
	
	private void Update ()
    {
        if (!_initialized) { return; }

        // Evaluate highest priority last.
	    for(int i=_activeBehaviors.Count-1; i>=0; i--)
        {
            _activeBehaviors[i].Evaluate();
        }
	}

    private void LoadBehaviors()
    {
        // Load all possible behaviors here.
        PlayerCloseFollow b1 = new PlayerCloseFollow();
        b1.Initialize("PlayerCloseFollow");
        _allBehaviors.Add(b1.Name, b1);
    }

    // Adds a new behavior in addition to existing behaviors
    public BaseCameraBehavior AddBehavior(string name, int priority=0)
    {
        if (!_allBehaviors.ContainsKey(name)) { return null; }
        BaseCameraBehavior bcb = _allBehaviors[name];
        bcb.Reset(priority);
        bool added = false;

        // Maintain Active behaviors in priority order.
        for(int i=0; i<_activeBehaviors.Count; i++)
        {
            if(bcb.Priority < _activeBehaviors[i].Priority )
            {
                _activeBehaviors.Insert(i, bcb);
                added = true;
            }
        }
        if(!added)
        {
            _activeBehaviors.Add(bcb);
        }
        return bcb;
    }

    // Removes the specified Behavior 
    public void RemoveBehavior(string n)
    {
        for(int i = 0; i<_activeBehaviors.Count; i++)
        {
            if(_activeBehaviors[i].Name == n)
            {
                _activeBehaviors.RemoveAt(i);
                break;
            }
        }
    }

    // Removes all pre-existing behaviors and adds the specified one.
    public BaseCameraBehavior SetBehavior(string n, int priority=0)
    {
        if (!_allBehaviors.ContainsKey(n)) { return null; }

        _activeBehaviors.Clear();
        BaseCameraBehavior bcb = _allBehaviors[n];
        bcb.Reset(priority);
        _activeBehaviors.Add(bcb);
        return bcb;
    }
}
