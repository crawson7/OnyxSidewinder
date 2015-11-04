using UnityEngine;
using System.Collections;
using System;

public class PlanetFollow : BaseCameraBehavior
{
    public PlanetFollow() { }
    //private Vector3 _center = new Vector3(0f, 0f, 0f);
    //private Vector3 _newCenter = new Vector3(0f, 0f, 0f);
    private Vector2 _zoomMax = new Vector2(30f,0f);
    private Vector2 _zoomMin = new Vector2(5f,0f);
    private Vector2 _zoom;
    private Vector2 _currentZoom;

    public override void Evaluate()
    {
        if(Game.Instance.ActivePlanet == null) { return; }

        Vector3 pos = Game.Instance.ActivePlanet.Center;
        pos.z = -10;
        if (Game.Instance.Player.Orbiting)
            _zoom.x = Game.Instance.Player.Speed*0.7f;
        if (_zoom.x > _zoomMax.x)
            _zoom = _zoomMax;
        else if (_zoom.x < _zoomMin.x)
            _zoom = _zoomMin;

        _currentZoom.x = CameraManager.Instance.GameCam.Cam.orthographicSize;
        CameraManager.Instance.GameCam.Cam.orthographicSize = iTween.Vector2Update(_currentZoom, _zoom, 1).x;
        
        //CameraManager.Instance.GameCam.Cam.orthographicSize = Game.Instance.Player.Speed;
        CameraManager.Instance.GameCam.Obj.position = iTween.Vector3Update(CameraManager.Instance.GameCam.Obj.position, pos, 1);
    }

    public override bool Initialize(string name)
    {
        Name = name;
        return true;
    }
}
