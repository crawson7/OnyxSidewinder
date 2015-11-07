using UnityEngine;
using System.Collections;

public static class Logger{

    public enum Type
    {
        Any,
        Info,
        Input
    }

    public static bool Active = false;

	// Log Level indicates the level of severity of the log statement.
	// Logger will display all messages that are greater than or equal to the specified level.
	// ---------------------------------------------------------------------------------------
	// Severity levels are as follows:
	// 0 = Update Statements (Use sparingly)
	// 1 = Game flow informational statements.
	// 2 = Game events or Inputs
	// 3 = Warnings. Things that might be problem indicators but to not break game flow.
	// 4 = Errors. problems in the code that may cause the game to not funciton properly.
	// 5 = Critical Errors. Failures to load or initialize, or conditions that nessesarily break the game.
	// ---------------------------------------------------------------------------------------
	public static int LogLevel;


	public static bool Initialize(int i)
    {
		LogLevel = i;
        Active = true;
        return true;
    }

    public static void Log(string s)
    {
        Debug.Log(s);
    }

    public static void Log(string s, int level, Type t = Type.Any)
    {
		if(level >= LogLevel)
		{
			if(level>=4)
			{Debug.LogError(s);}
			else if(level>=3)
			{Debug.LogWarning(s);}
			else
			{Debug.Log(s);}
		}
    }
}
