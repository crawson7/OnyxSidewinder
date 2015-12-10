using UnityEngine;
using System.Collections;

public class PlanetPopper : PlanetBase {

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
		Logger.Log("Collided with Popper", 1);
		Global.Level.Planets.RemovePlanet(this);
	}

	public override void HandleGravityCollide()
	{
		Logger.Log("Collided with Poppers Gravity");
		_orbitActive = true;
		Game.Instance.HandleGravityCollide(this);
	}
		
}
