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

using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "f00754b18618b55d4a77203e50d2849b603d5235")]
public class Buoyancy : Component
{
	[ShowInEditor]
	Node WaterNode, BoatNode;

    [ShowInEditor]
    Node CenterOfMass;

    public ivec2 GridSize;
	public float _Buoyancy, LinearDamp, AngularDamp;

    internal class VolumePart
    {
        public dvec3 Center;
        public float WaterHeight;
    }

    dvec3 Size;
	mat3 Rotation;

	bool isUnderWater(float Height) => Height != 0;
    double GetHWidth() => Size.x * 0.5;
	double GetHHeight() => Size.z * 0.5;
	double GetHDepth() => Size.y * 0.5;
	double GetWaterVolume(float HeightWater) => Size.x * Size.y * HeightWater;
	dvec3 GetAnchorPoint(dvec3 Center) => Center - new dvec3(Rotation.AxisZ) * GetHHeight();


	List<VolumePart> Parts = new();
    ObjectWaterGlobal Water;
	BodyRigid Body;
	Shape Shape;

    Gui GUI;
	WidgetSlider WaterSlider;


	private void Init()
	{
        // write here code to be called on component initialization
       
		GUI = Gui.GetCurrent();

        WaterSlider = new WidgetSlider(0, 20, 10);
        WaterSlider.Width = 150;
        WaterSlider.SetPosition(50, 30);
        GUI.AddChild(WaterSlider, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        Water = WaterNode as ObjectWaterGlobal;
		Body = BoatNode.ObjectBodyRigid;
		Shape = Body.GetShape(0);
		Parts.Capacity = GridSize.x * GridSize.y;
		for (int i = 0; i < Parts.Capacity; i++) { Parts.Add(new VolumePart()); }
    }
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		
		Size.x = Shape.Area.x / GridSize.x;
		Size.y = Shape.Area.y / GridSize.y;
		Size.z = Shape.Area.z;

		Rotation = new mat3(Shape.Transform);
		dmat4 T = Shape.Transform;
		dvec3 S_Size = Shape.Area;
		dvec3 Start = T.Translate - T.AxisX * S_Size.x * 0.5
								  - T.AxisY * S_Size.y * 0.5;

		for (int i = 0; i < Parts.Count; ++i)
		{
			int y = i / GridSize.x;
			int x = i / GridSize.y;

			Parts[i].Center = Start 
				+ Rotation.AxisX * (x * Size.x * GetHWidth())
				+ Rotation.AxisY * (y * Size.y * GetHDepth());
		}

		for (int i = 0;i < Parts.Count; ++i)
		{
			dvec3 Pos;
			Pos.x = GetAnchorPoint(Parts[i].Center).x;
			Pos.y = GetAnchorPoint(Parts[i].Center).y;
			Pos.z = 0;

            float h = Water.FetchHeight(Pos);
			float z = (float)GetAnchorPoint(Parts[i].Center).z;

            if (z < h)
			{
				Parts[i].WaterHeight = MathLib.Clamp(h - z, 0, (float)Size.z);
			}
			else { Parts[i].WaterHeight = 0; }
		}

		for(int i = 0; i < Parts.Count; ++i)
		{
			dmat4 Transform = new();
			Transform.Set(Rotation, Parts[i].Center);
			Visualizer.RenderPoint3D(GetAnchorPoint(Parts[i].Center), 0.3f, vec4.WHITE);

			dvec3 VolSize = new(Size.x, Size.y, Parts[i].WaterHeight);
			dmat4 VolTran = new(Rotation, GetAnchorPoint(Parts[i].Center) + Rotation.AxisZ * VolSize.z * 0.5);

			if (isUnderWater(Parts[i].WaterHeight))
			{
				Visualizer.RenderVector(
					GetAnchorPoint(Parts[i].Center),
                    GetAnchorPoint(Parts[i].Center) + Rotation.AxisZ * VolSize.z,
					vec4.RED
					);
				Visualizer.RenderBox((vec3)VolSize, VolTran, vec4.BLACK);
			}
		}

	}

	private void UpdatePhysics()
	{
		//float WaterVal = 0;
		//float AllVol = Shape.Volume;

		//for (int i = 0; i < Parts.Count; ++i)
		//{
		//	if (!isUnderWater(Parts[i].WaterHeight)) { continue; }

		//	dvec3 Force = new dvec3(Physics.Gravity * GetWaterVolume(Parts[i].WaterHeight) * _Buoyancy);
		//	Force /= Parts.Count;
		//	Body.AddForce((vec3)Force);

		//	dvec3 AxisR = new(Rotation.AxisZ * Parts[i].WaterHeight - Body.CenterOfMass);
		//	dvec3 Radius = new(GetAnchorPoint(Parts[i].Center) + AxisR);

		//	dvec3 Torque = MathLib.Cross(Radius, Force);
		//	Body.AddTorque((vec3)Torque);
		//	WaterVal += (float)GetWaterVolume(Parts[i].WaterHeight);
		//}

		//float Coeff = 0;
		//if (AllVol > 0)
		//{
		//	Coeff = WaterVal / AllVol;
		//}

		//Body.AddLinearImpulse(-Body.LinearVelocity * LinearDamp * Coeff * Body.Mass);
		//Body.AddAngularImpulse((-Body.AngularVelocity * AngularDamp * Coeff) * MathLib.Inverse(Body.IWorldInertia));

	}


    private void Shutdown()
	{
		GUI = Gui.GetCurrent();
        if (GUI.IsChild(WaterSlider) == 1) { GUI.RemoveChild(WaterSlider); WaterSlider.DeleteLater(); }
    }
}