using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateLevelMenu : MonoBehaviour
{
    public static string createName;
    private static SceneController _sceneControl;

    [MenuItem("Sidewinder/New Level")]
    static void StartCreate()
    {
        ShowPopupExample.Init();
    }

    public static void NewLevel()
    {
        Debug.Log("Creating a new Level");
        EditorApplication.NewEmptyScene();

        Debug.Log("Building Objects");
        GameObject sceneControl_GO = new GameObject("SceneController");
        _sceneControl = sceneControl_GO.AddComponent<SceneController>();
        _sceneControl.Name = createName;

        GameObject levelSettings_GO = new GameObject("LevelSettings");
        levelSettings_GO.transform.SetParent(sceneControl_GO.transform, false);
        LevelSettingsEditor settings = levelSettings_GO.AddComponent<LevelSettingsEditor>();

        GameObject levelBounds_GO = new GameObject("LevelBounds");
        levelBounds_GO.transform.SetParent(sceneControl_GO.transform, false);
        LevelBoundsIndicator bounds = levelBounds_GO.AddComponent<LevelBoundsIndicator>();
        bounds.Initialize();

        GameObject startPosition_GO = new GameObject("PlayerStartPosition");
        startPosition_GO.transform.SetParent(sceneControl_GO.transform, false);
        SpriteRenderer startSR = startPosition_GO.AddComponent<SpriteRenderer>();
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/PlayerRef.png") as Sprite;
        if (s != null) {
            startSR.sprite = s;
            startSR.sortingOrder = 10;
        }
        else { Debug.LogError("Sprite for Player Not Found."); }
        
        // TODO: Load Environment Prefab from Resources.

        settings.Initialize(levelBounds_GO, startPosition_GO);
        _sceneControl.LevelSettings = settings;
    }

    [MenuItem("Sidewinder/Save Level")]
    static void SaveLevel()
    {
        Debug.Log("Saving the Level");
        EditorApplication.SaveScene("Assets/Scenes/Levels/" + _sceneControl.Name + ".unity");

        // TODO: Update Levels JSON file.
    }
}


class ShowPopupExample : EditorWindow
{
    string className = "Level Name";

    [MenuItem("Example/Delete Components in Selection")]
		public static void Init()
    {
        var window = new ShowPopupExample();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 50);
        window.ShowUtility();
    }

    void OnGUI() {
        className = EditorGUILayout.TextField("Level Name:", className);
        if (GUILayout.Button("Do Thing!"))
        {
            EditorApplication.delayCall += () => {
                CreateLevelMenu.createName = className;
                CreateLevelMenu.NewLevel();
            };
            Debug.Log("The Name is: " + className);
            this.Close();
        }
    }

}
