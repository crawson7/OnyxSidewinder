using UnityEngine;
using System.Collections;
using System;

public class PlayerCloseFollow : BaseCameraBehavior
{
    public PlayerCloseFollow() { }

    public override void Evaluate()
    {
        Vector3 pos = Game.Instance.Player.Position;
        pos.z = -10;
        CameraManager.Instance.GameCam.Obj.position = pos;
    }

    public override bool Initialize(string name)
    {
        Name = name;
        return true;
    }
}
