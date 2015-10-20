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

    public static bool Initialize()
    {
        Active = true;
        return true;
    }

    public static void Log(string s)
    {
        Debug.Log(s);
    }

    public static void Log(string s, int level, Type t = Type.Any)
    {
        Debug.Log(s);
    }
}
