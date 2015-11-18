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
    private Vector3 target;
    private Vector3 cameraPos;
    private FloatRingBuffer cameraZoomBuffer = new FloatRingBuffer(3);

    public override void Evaluate()
    {
        if(Game.Instance.ActivePlanet == null) { return; }

        Vector3 planetPos = Game.Instance.ActivePlanet.Center;
        Vector3 playerPos = Game.Instance.Player.Position;


        planetPos.z = -10;
        playerPos.z = -10;
        //if (Game.Instance.Player.Orbiting)
           _zoom.x = Game.Instance.Player.Speed*1.25f;
        if (_zoom.x > _zoomMax.x)
            _zoom = _zoomMax;
        else if (_zoom.x < _zoomMin.x)
            _zoom = _zoomMin;

        cameraZoomBuffer.addValue(CameraManager.Instance.GameCam.Cam.orthographicSize);
        //GetAverage() is called every frame, and its very expensive. We only need to call it when the player collides with gravity,
        //because there's a speed change thats bizzarre and the ringbuffer smooths it out. Can we detect that and only use GetAverage()
        //when we need it?
        _currentZoom.x = cameraZoomBuffer.GetAverage(); 
        CameraManager.Instance.GameCam.Cam.orthographicSize =  iTween.Vector2Update(_currentZoom, _zoom, 1).x;

        //CameraManager.Instance.GameCam.Cam.orthographicSize = Game.Instance.Player.Speed;
        

        if(Game.Instance.Player.Orbiting)
        {
            target = iTween.Vector3Update(target, planetPos, 2f);// iTween.Vector3Update(playerPos, planetPos, 1f);
        }
        else
        {
            target = playerPos;
        }
        CameraManager.Instance.GameCam.Obj.position = iTween.Vector3Update(CameraManager.Instance.GameCam.Obj.position, target, 0.5f);

        if(Game.Instance.DebugCameraEnabled)
        {
            target.z = -2;
            Game.Instance.CameraTarget.transform.position = target;

            cameraPos = CameraManager.Instance.GameCam.Obj.position;
            cameraPos.z = -3;
            Game.Instance.CameraPosition.transform.position = cameraPos;

        }
    }

    public override bool Initialize(string name)
    {
        Name = name;
        return true;
    }

}
