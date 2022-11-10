using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "0310fa99a115a324f80c94335119cbf68d0852dd")]
public class StarMove : Component
{
	public Node Sun, Stars, Moon;

	private void Update()
	{
        // write here code to be called before updating each render frame

        Stars.SetWorldRotation(Stars.GetWorldRotation() * new quat(0, 0, Game.IFps));
        Sun.SetWorldRotation(Sun.GetWorldRotation() * new quat(Game.IFps, Game.IFps, 0));
        Moon.SetWorldRotation(Moon.GetWorldRotation() * new quat(Game.IFps * 0.5f, Game.IFps, 0));
    }
}