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
using System.Reflection.Metadata;
using Unigine;

[Component(PropertyGuid = "1703e40127f4add2944d16888103c920cbe1926c")]
public class BoatMovement : Component
{

    [ShowInEditor]
    Node Pod1, Pod2;

	Gui GUI;
    WidgetSlider BoatFBSlider;
    WidgetSlider BoatLRSlider;
    
    float Angle = 180;
    BodyRigid Body;

    private void Init()
	{
		// write here code to be called on component initialization

		GUI = Gui.GetCurrent();

        BoatFBSlider = new WidgetSlider(0, 20, 10); // [-10, 10] Speed
        BoatFBSlider.Width = 150;
        BoatFBSlider.SetPosition(50, 10);
        GUI.AddChild(BoatFBSlider, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        BoatLRSlider = new WidgetSlider(0, 10, 5); // [-5, 5] Speed
        BoatLRSlider.Width = 150;
        BoatLRSlider.SetPosition(50, 30);
        GUI.AddChild(BoatLRSlider, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        Body = node.ObjectBodyRigid;
    }
	
	private void UpdatePhysics()
	{

        vec3 NewL = vec3.ZERO;
        NewL.xyz += BoatFBSlider.Value - 10;

        Body.AddWorldImpulse(Body.WorldCenterOfMass,node.GetWorldDirection(MathLib.AXIS.Y) * Body.IMass * NewL * Physics.IFps);
        Body.AddWorldForce(Body.CenterOfMass - Pod1.WorldPosition, Pod1.GetWorldDirection(MathLib.AXIS.NY) * Body.IMass * NewL * Physics.IFps);
        Body.AddWorldForce(Body.CenterOfMass - Pod2.WorldPosition, Pod2.GetWorldDirection(MathLib.AXIS.NY) * Body.IMass * NewL * Physics.IFps);
        Pod1.SetWorldRotation(new quat(0, 0, Angle - ((BoatLRSlider.Value - 5) * 15)));
        Pod2.SetWorldRotation(new quat(0, 0, Angle - ((BoatLRSlider.Value - 5) * 15)));

    }

    private void Shutdown()
    {
        GUI = Gui.GetCurrent();
        if (GUI.IsChild(BoatFBSlider) == 1) { GUI.RemoveChild(BoatFBSlider); BoatFBSlider.DeleteLater(); }
        if (GUI.IsChild(BoatLRSlider) == 1) { GUI.RemoveChild(BoatLRSlider); BoatLRSlider.DeleteLater(); }
    }
}