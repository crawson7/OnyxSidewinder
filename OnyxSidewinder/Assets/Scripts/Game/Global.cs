using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Global
{
    private static Dictionary<PlanetType, PlanetTypeData> _planetTypeData;

    public static Level Level{get{return Game.Instance.CurrentLevel;}}
	public static PlayerController Player{get{return Game.Instance.Player;}}

    public static PlanetTypeData GetPlanetTypeData(PlanetType type)
    {
        // Load the Data if it does not yet exist
        if(_planetTypeData == null)
        {
            _planetTypeData = new Dictionary<PlanetType, PlanetTypeData>();
            _planetTypeData.Add(PlanetType.Chomper, new PlanetTypeData(PlanetType.Chomper, "big_guy"));
            _planetTypeData.Add(PlanetType.Bouncer, new PlanetTypeData(PlanetType.Bouncer, "flapper"));
            _planetTypeData.Add(PlanetType.Popper, new PlanetTypeData(PlanetType.Popper, "lil_guy"));
        }

        // Return the data type or popper Type if not found.
        if(_planetTypeData.ContainsKey(type))
        {
            return _planetTypeData[type];
        }
        return _planetTypeData[PlanetType.Popper];
    } 
}
