using UnityEngine;
using System.Collections;

public static class Global
{
	public static Level Level{get{return Game.Instance.CurrentLevel;}}
	public static PlayerController Player{get{return Game.Instance.Player;}}
}
