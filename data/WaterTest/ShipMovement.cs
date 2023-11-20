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

[Component(PropertyGuid = "8ed0c2ec80f4ee73f586b5277147848d7825f448")]
public class ShipMovement : Component
{

	[ShowInEditor]
	Node Pod1, Pod2;

    Gui GUI;
    WidgetSlider BoatFBSlider;
    WidgetSlider BoatLRSlider;

    BodyRigid Body;

    private void Init()
    {
        // write here code to be called on component initialization

        GUI = Gui.GetCurrent();

        BoatFBSlider = new WidgetSlider(0, 20, 10); // [-10, 10] Speed
        BoatFBSlider.Width = 350;
        BoatFBSlider.SetPosition(50, 10);
        GUI.AddChild(BoatFBSlider, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        BoatLRSlider = new WidgetSlider(0, 10, 5); // [-5, 5] Speed
        BoatLRSlider.Width = 350;
        BoatLRSlider.SetPosition(50, 30);
        GUI.AddChild(BoatLRSlider, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

    }

    private void UpdatePhysics()
    {
        Body = node.ObjectBodyRigid;

        vec3 NewL = vec3.ZERO;
        float Angle = 180;
        NewL.xyz += BoatFBSlider.Value - 10;
        Angle -= ((BoatLRSlider.Value - 5) * 15);

        Body.AddLinearImpulse((Pod1.GetWorldDirection(MathLib.AXIS.NY) + Pod2.GetWorldDirection(MathLib.AXIS.NY)) * NewL * Body.Mass);
        Pod1.SetWorldRotation(new quat(0, 0, Angle));
        Pod2.SetWorldRotation(new quat(0, 0, Angle));
    }

    private void Shutdown()
    {
        GUI = Gui.GetCurrent();
        if (GUI.IsChild(BoatFBSlider) == 1) { GUI.RemoveChild(BoatFBSlider); BoatFBSlider.DeleteLater(); }
        if (GUI.IsChild(BoatLRSlider) == 1) { GUI.RemoveChild(BoatLRSlider); BoatLRSlider.DeleteLater(); }
    }
}