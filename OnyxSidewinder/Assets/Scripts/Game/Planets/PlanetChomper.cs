﻿using UnityEngine;
using System.Collections;

public class PlanetChomper : PlanetBase {

    public override void Initialize(float body, float gravity, PlanetType type)
    {
        base.Initialize(body, gravity, type);
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override void SetType(PlanetType type)
    {
        // Modify the behavior and skin to match the type
        // Set the Sprite to match.
        // Reset the animations and state
    }

    public override void HandleBodyCollide()
    {

    }

    public override void HandleGravityCollide()
    {


    }

    public override void HandleOrbit()
    {

    }
}
