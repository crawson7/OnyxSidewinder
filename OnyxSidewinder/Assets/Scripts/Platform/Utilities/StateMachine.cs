using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void StateDelegate();

public class StateMachine
{
	private string _name;
	private string _lastState = "";	
	private State _currentState = null;
	private string nextState = "";
    private bool _active;
    private Dictionary<string, State> _stateList = new Dictionary<string, State>();
    
	public string CurrentState{get{return (_currentState!=null)? _currentState.Name : "";}}
	public string LastState{get{return _lastState;}}
    public string Name { get{return _name; }}

	public StateMachine(string name)
	{
		_name = name;
		GameControl.Instance.RegisterStateMachine(this);
	}

    public void Terminate()
    {
        GameControl.Instance.RemoveStateMachine(this);
    }

	public State AddState(string state)
	{
		if(state == "" || _stateList.ContainsKey(state))
		{	
            Logger.Log("Error creating State " + state + ": must have a unique name.", 2); 
            return null;
        } 

		State newState = new State(state);
		_stateList.Add(_name, newState);		
		return newState;        
	}
	
    public void RemoveState(string state)
    {
        if(_stateList.ContainsKey(state))
        {
            _stateList.Remove(state);
        }
    }
    
	public State GetState(string state)
	{
        if(_stateList.ContainsKey(state))
        {
		  return _stateList[state];
        }
        return null;
	}

	public void Start(string beginState)
	{
        _active = true;
        if(CanSetTo(beginState))
        {
		  nextState = beginState;
		  SetStateInternal();
        }
        else
        {
            _active = false;
            Logger.Log("Error Starting State Machine: " + _name, 4);
        }
	}
    
    public bool IsValid(string state)
    {
        return _stateList.ContainsKey(state);
    }

    public void Stop()
    {
        if(_active && _currentState != null && _currentState.OnExit != null)
		{	
            _currentState.OnExit();
        }
                    
        _active = false;
        _currentState = null;
    }
	
	public void SetTo(string toState)
	{
		if(CanSetTo(toState))
        {
            nextState = toState;
            SetStateInternal();		
        }
	}

    public void ReturnToLastState()
    {
        if(_lastState == null || _lastState == "")
        {
            Logger.Log("There is no last state to switch to.", 3);
            return;
        }
        
        if(CanSetTo(_lastState))
        { 
            nextState = _lastState;
            SetStateInternal();	
        }
    }
          
    public void Update()
	{
		if(!_active){return;}
		
		if(_currentState != null && _currentState.OnUpdate != null)
		{_currentState.OnUpdate();}
	}
           
           
    #region INTERNAL HELPER METHODS
    private bool CanSetTo(string toState)
    {
        if(!_active)
        { 
            Logger.Log("This State machine " + _name + " is not active.", 3); 
            return false;
        }

        if(!_stateList.ContainsKey(nextState))
		{
            Logger.Log("The State " + nextState + " does not exist.");
			return false;
		}
        
		if(_currentState.Name == toState)
        { 
            Logger.Log("The State Machine is already in " + _currentState.Name + " state."); 
            return false;
        }
        return true;
    }
    
	private bool SetStateInternal()
	{
		if(_currentState != null)
		{	
			if(_currentState.OnExit != null)
			{_currentState.OnExit();}

			_currentState.ProcessLinkTo(nextState);
			_lastState = _currentState.Name;
		}

		_currentState = _stateList[nextState];
		nextState = "";
		Logger.Log(_name + " - Entering State: " + _currentState.Name, 0);
		
		if(_currentState != null && _currentState.OnEnter != null)
		{_currentState.OnEnter();}
		
		return true;
	} 
    #endregion
}


public class State
{
	private string _name = "";
	private Dictionary<string, StateDelegate> links = new Dictionary<string, StateDelegate>();
 	
    public StateDelegate OnEnter = null;
	public StateDelegate OnUpdate = null;
	public StateDelegate OnExit = null;
    public string Name{get{return _name;}}
    
	public State(string name)
	{
		_name = name;
	}
	
	public void AddLinkTo(string toState, StateDelegate logic)
	{
		if(logic == null && !links.ContainsKey(toState))
		{Debug.Log("Add StateLink Error"); return;}

		links.Add (toState, logic);
	}

	public void ProcessLinkTo(string next)
	{
		if(links.ContainsKey(next))
		{	links[next]();}
	}
}