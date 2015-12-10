using UnityEngine;
using System.Collections;

public class PlanetBouncer : PlanetBase {

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
		Logger.Log("Collided with Bouncer", 1);
		// TODO: Make the player bounce back away from the planet.
	}

	public override void HandleGravityCollide()
	{
		Logger.Log("Collided with Bouncers Gravity");
		_orbitActive = true;
		Game.Instance.HandleGravityCollide(this);
	}
		
}
