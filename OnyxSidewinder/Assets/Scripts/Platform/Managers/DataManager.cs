using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using System;

public static class DataManager
{
	private static string PATH = Application.persistentDataPath + "/Data/";
	private static string RESOURCES = "Data/";

	public static bool Initialize()
	{
		return true;
	}


	#region Public Access Methods
	public static bool Load<T>(out T data, bool resourcesFirst=false)
	{
		// Extract the file name for this data.
		data = default(T);
		Type parameterType = typeof(T);
		string fileName = parameterType.ToString();
		string filePath = PATH + fileName + ".txt";
		string dataString;

		// Try Loading from DataPath
		if(!resourcesFirst && File.Exists(filePath))
		{
			GetDataFromDataPath(filePath, out dataString);
		}
		else
		{
			// Try Loading from Resources
			if(!GetDataFromResources("Data/" + fileName, out dataString))
			{
				if(resourcesFirst && File.Exists(filePath))
				{
					GetDataFromDataPath(filePath, out dataString);
				}
				else
				{
					Logger.Log ("Could not Load " + fileName + " Data From Resources or Data Path", 3); 
					CreateNewFile(fileName, "");
					return false;
				}
			}
		}

		// Deserialize the JSON file to the specified object.
		T dataOut = JsonReader.Deserialize<T>(dataString);
		if(dataOut == null)
		{
			Logger.Log("Error deserializing data file " + fileName, 4);
			return false;
		}

		Logger.Log(fileName + " successfully loaded.", 1);
		data = dataOut;
		return true;
	}

	public static bool Save<T>(T data)
	{
		// This will save a file to the Application Path only.
		Type parameterType = typeof(T);
		if(!IsSerializable(parameterType)){return false;}
	
		// Extract the file name for this data
		string fileName = parameterType.ToString();
		string filePath = PATH + fileName + ".txt";

		// Serialize the hata.
		string dataString = JsonWriter.Serialize(data);
		if(!Directory.Exists(PATH)){Directory.CreateDirectory(PATH);}

		// Write out the data
		var writer = new StreamWriter(filePath);
		writer.Write(dataString);
		writer.Close();
		Logger.Log(fileName + " successfully saved.", 1);
		return true;
	}

	public static bool SaveToResources<T>(T data)
	{
		// Saving to Resources can only be done in Unity Editor.
		#if UNITY_EDITOR
		Type parameterType = typeof(T);
		if(!IsSerializable(parameterType)){return false;}

		string filePath = Application.dataPath + "/Resources/Data/" + parameterType.ToString() + ".txt";
        Debug.Log("Saving File to: " + filePath);
		string dataString = JsonWriter.Serialize(data);
		StreamWriter writer = new StreamWriter(filePath);
		writer.Write(dataString);
		writer.Close();
		return true;
		#else
		Logger.Logger("Cannon save data to Resources. This can only be done in Unity_Editor.", 4);
		return false;
		#endif
	}
	#endregion


	#region Private Helper Methods
	private static void CreateNewFile(string file, string data)
	{
		if(!Directory.Exists(PATH))
		{Directory.CreateDirectory(PATH);}

		File.WriteAllText(PATH + file + ".txt", data);
	}

	private static bool GetDataFromResources(string path, out string data)
	{
		TextAsset fileData= Resources.Load(path) as TextAsset;
		if(fileData==null)
		{
			data = "";
			return false;
		}
		data = fileData.text;
		return true;
	}
	
	private static void GetDataFromDataPath(string path, out string data)
	{
		var streamReader = new StreamReader(path);
		data = streamReader.ReadToEnd();
		streamReader.Close();
	}

	private static bool IsSerializable(Type type)
	{
		System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
		for(int i=0; i<attrs.Length; i++)
		{
			if(attrs[i].TypeId.ToString() == "System.SerializableAttribute")
			{
				return true;
			}
		}
		Logger.Log("Class " + type.ToString() + " is NOT Serializable.", 1);
		return false;
	}
	#endregion
}
