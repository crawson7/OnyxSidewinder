﻿using UnityEngine;
using System.Collections;
using System;

public class ZoomWithSpeed : BaseCameraBehavior
{
    public ZoomWithSpeed() { }

    public override void Evaluate()
    {
        CameraManager.Instance.GameCam.Cam.orthographicSize = Game.Instance.Player.Speed;
    }

    public override bool Initialize(string name)
    {
        Name = name;
        return true;
    }
}
