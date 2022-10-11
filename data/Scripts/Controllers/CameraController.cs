using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "03a31444606e12d3d420fa1691e66a7dd9c529b8")]
public class CameraController : Component
{
    Player MainPlayer;
    public bool Invert_Y = true, Invert_X = true;

    [ParameterSlider(Min = 0.1f, Max = 20f)]
    [ShowInEditor]
    private float Radius = 6, Rotation_Speed_Horizontal = 0.5f, Rotation_Speed_Vertical = 0.5f, HeightOffset = 1;

    [ParameterSlider(Min = 0f, Max = 5f)]
    [ShowInEditor]
    private float Maximum_Height = 3f, Minimum_Height = 0.8f;

    public enum FollowObject { SELF, CUSTOM }
    public FollowObject ObjectToFollow = FollowObject.SELF;

    [ParameterCondition(nameof(ObjectToFollow), (int)FollowObject.CUSTOM)]
    public Node ObjectFollow;

    int InvertY, InvertX;
    float Angle, Height;
    dvec3 NPoint = new vec3();
    vec2 MousePos = new vec2();

    private void Init()
    {
        // write here code to be called on component initialization

        if (ObjectFollow == null || ObjectToFollow == FollowObject.SELF) { ObjectFollow = node; }

        if (Game.Player != null) MainPlayer = Game.Player;
        else
        {
            MainPlayer = new PlayerSpectator();
            Game.Player = MainPlayer;
            MainPlayer.Controlled = false;
        }


        if (Invert_Y) InvertY = 1; else InvertY = -1;
        if (Invert_X) InvertX = 1; else InvertX = -1;
        Angle = 0;
    }

    private void Update()
    {
        // write here code to be called before updating each render frame

        MousePos = Input.MouseDeltaPosition;

        Angle += InvertX * MousePos.x * Game.IFps * Rotation_Speed_Horizontal;
        Height = MathLib.Clamp(Height += InvertY * MousePos.y * Game.IFps * Rotation_Speed_Vertical, Minimum_Height, Maximum_Height);

        NPoint.x = MathLib.Cos(Angle) * Radius;
        NPoint.y = MathLib.Sin(Angle) * Radius;
        NPoint.z = Height + HeightOffset;

        NPoint += ObjectFollow.WorldPosition;

        MainPlayer.WorldPosition = NPoint;
        MainPlayer.WorldLookAt(ObjectFollow.WorldPosition);

    }
}