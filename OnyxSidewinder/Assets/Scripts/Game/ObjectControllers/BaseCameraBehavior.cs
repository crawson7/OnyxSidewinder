using UnityEngine;
using System.Collections;

public abstract class BaseCameraBehavior
{
    public string Name;
    public int Priority;

    public abstract bool Initialize(string name);
    public virtual void Reset(int priority) { Priority = priority; }
    public abstract void Evaluate();
}
