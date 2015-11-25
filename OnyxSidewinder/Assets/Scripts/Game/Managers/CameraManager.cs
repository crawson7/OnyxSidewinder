using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    #region Singleton Access
    private static CameraManager _instance;

    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Cameras").GetComponent<CameraManager>();
            }
            return _instance;
        }
    }
    #endregion

    public CameraController GameCam;
    public CameraController UICam;

    public bool Initialize()
    {
        if (!GameCam.Initialize()) { return false; }
        if (!UICam.Initialize()) { return false; }
        return true;
    }
}
