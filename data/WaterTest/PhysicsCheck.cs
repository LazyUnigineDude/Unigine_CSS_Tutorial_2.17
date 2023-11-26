/* Copyright (C) 2005-2023, UNIGINE. All rights reserved.
*
* This file is a part of the UNIGINE 2 SDK.
*
* Your use and / or redistribution of this software in source and / or
* binary form, with or without modification, is subject to: (i) your
* ongoing acceptance of and compliance with the terms and conditions of
* the UNIGINE License Agreement; and (ii) your inclusion of this notice
* in any version of this software that you use or redistribute.
* A copy of the UNIGINE License Agreement is available by contacting
* UNIGINE. at http://unigine.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "3bb94f0f5b0598b7a5b03df046f6c8d3df354b89")]
public class PhysicsCheck : Component
{
	[ShowInEditor]
	Node Left, Right;
	BodyRigid body;
	quat Rotation;
	float Angle = 180;

	private void Init()
	{
		// write here code to be called on component initialization

		body = node.ObjectBodyRigid;
    }
	
	private void Update()
	{
		// write here code to be called before updating each render frame

		Visualizer.RenderVector(Left.WorldPosition,  Left.WorldPosition +  Left.GetWorldDirection(MathLib.AXIS.Y),  vec4.RED, 0.5f);
		Visualizer.RenderVector(Right.WorldPosition, Right.WorldPosition + Right.GetWorldDirection(MathLib.AXIS.Y), vec4.RED, 0.5f);

        if (Input.IsKeyPressed(Input.KEY.A)) { Angle -= 1f; }
		if (Input.IsKeyPressed(Input.KEY.D)) { Angle += 1f; }

        if (Input.IsKeyPressed(Input.KEY.W)) { body.AddWorldForce(Left. WorldPosition, Left. GetWorldDirection(MathLib.AXIS.NY) * 50); }
        if (Input.IsKeyPressed(Input.KEY.W)) { body.AddWorldForce(Right.WorldPosition, Right.GetWorldDirection(MathLib.AXIS.NY) * 50); }

		Left .SetWorldRotation(new quat(0, 0, Angle));
		Right.SetWorldRotation(new quat(0, 0, Angle));
    }
}