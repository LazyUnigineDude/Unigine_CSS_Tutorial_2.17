using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "24c6da30a42727cf0f7f72e205bfd4d9bf2c3864")]
public class InputController : Component
{
    [ShowInEditor]
    private int Speed;

    private quat left, right;

    void Init()
    {
        left = new quat(0, 0, 1);
        right = new quat(0, 0, -1);
    }

    private void Update()
    {
        if (Input.IsKeyPressed(Input.KEY.W)) { node.WorldPosition = node.WorldPosition + node.GetWorldDirection(MathLib.AXIS.Y) * Game.IFps * Speed; }
        if (Input.IsKeyPressed(Input.KEY.S)) { node.WorldPosition = node.WorldPosition + node.GetWorldDirection(MathLib.AXIS.NY) * Game.IFps * Speed; }
        if (Input.IsKeyPressed(Input.KEY.A)) { node.WorldRotate(left); }
        if (Input.IsKeyPressed(Input.KEY.D)) { node.WorldRotate(right); }


        if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT)) { Log.Message("LMB Pressed\n"); }
    }
}