using UnityEngine;
using System.Collections;

public class PlanetChomper : PlanetBase {

    public override void Initialize(PlanetData data)
    {
        base.Initialize(data);
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override void SetType(PlanetType type)
    {
		base.SetType(type);
    }

	public override void HandleBodyCollide()
	{
		Logger.Log("Collided with Chomper", 1);
		Global.Player.Kill();
		Game.Instance.End(false);
	}

	public override void HandleGravityCollide()
	{
		Logger.Log("Collided with Chompers Gravity");
		_orbitActive = true;
		Game.Instance.HandleGravityCollide(this);
	}
		
}
