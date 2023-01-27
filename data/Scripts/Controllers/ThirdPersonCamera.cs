using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "fd977b057ba130a07c9e2b7fd532c2d250248409")]
public class ThirdPersonCamera : Component
{
    PropertyParameter Normal, Aiming;
    public Property ThirdPersonCameraProperty;
    public float SpeedOfChange = 5;
    public bool isAiming = false, InvertHorizontal, InvertVertical;

    float Angle = 0, CamRadius = 0, Radius = 0, Height = 0, HeightOffset = 0, RotationSpeedH = 0, RotationSpeedV = 0, Weight = 0;
    vec2 HeightConstraints = new();
    dvec3 Npoint, Cpoint;
    int InvertX = 1, InvertY = 1;

    public Node Following, Camera;
    enum CAMERASTATE { Normal, Aiming }
    CAMERASTATE PREV_STATE, STATE;

    private void Init()
    {
        Normal = ThirdPersonCameraProperty.GetParameterPtr(0).GetChild(0);
        Aiming = ThirdPersonCameraProperty.GetParameterPtr(0).GetChild(1);

        PREV_STATE = CAMERASTATE.Aiming;
        STATE = CAMERASTATE.Normal;
        Invert();
    }

    private void Update()
    {
        if (isAiming) { STATE = CAMERASTATE.Aiming; } else STATE = CAMERASTATE.Normal;
        if (STATE != PREV_STATE) { Weight = 0; PREV_STATE = STATE; }

        LerpLayer(STATE);
        UpdateCamera();
    }

    void Invert()
    {
        if (InvertHorizontal) InvertX = 1; else InvertX = -1;
        if (InvertVertical) InvertY = -1; else InvertY = 1;
    }

    void UpdateCamera()
    {
        ivec2 Mousepos = Input.MouseDeltaPosition;

        Angle += InvertX * Mousepos.x * Game.IFps * RotationSpeedH;
        Height = MathLib.Clamp(Height += InvertY * Mousepos.y * Game.IFps * RotationSpeedV,
                               HeightConstraints.x * MathLib.DEG2RAD,
                               HeightConstraints.y * MathLib.DEG2RAD);

        Npoint.x = MathLib.Cos(Angle) * Radius;
        Npoint.y = MathLib.Sin(Angle) * Radius;
        Npoint.z = HeightOffset;
        Npoint += Following.WorldPosition;

        Cpoint.x = MathLib.Cos(Angle - 20) * CamRadius;
        Cpoint.y = MathLib.Sin(Angle - 20) * CamRadius;
        Cpoint.z = MathLib.Tan(Height);
        Cpoint += Npoint;

        Camera.WorldPosition = Cpoint;
        Camera.WorldLookAt(Npoint);
    }

    void LerpLayer(CAMERASTATE STATE)
    {
        Weight = MathLib.Clamp(Weight += Game.IFps * SpeedOfChange, 0f, 1f);

        switch (STATE)
        {
            case CAMERASTATE.Normal: if (Weight != 1) LerpValues(Aiming, Normal); break;
            case CAMERASTATE.Aiming: if (Weight != 1) LerpValues(Normal, Aiming); break;
            default: break;
        }
    }

    void LerpValues(PropertyParameter PREV_STATE, PropertyParameter STATE)
    {
        Radius = MathLib.Lerp(PREV_STATE.GetChild(0).ValueFloat, STATE.GetChild(0).ValueFloat, Weight);
        CamRadius = MathLib.Lerp(PREV_STATE.GetChild(1).ValueFloat, STATE.GetChild(1).ValueFloat, Weight);
        RotationSpeedH = MathLib.Lerp(PREV_STATE.GetChild(2).ValueFloat, STATE.GetChild(2).ValueFloat, Weight);
        RotationSpeedV = MathLib.Lerp(PREV_STATE.GetChild(3).ValueFloat, STATE.GetChild(3).ValueFloat, Weight);
        HeightOffset = MathLib.Lerp(PREV_STATE.GetChild(4).ValueFloat, STATE.GetChild(4).ValueFloat, Weight);
        HeightConstraints = MathLib.Lerp(PREV_STATE.GetChild(5).ValueVec2, STATE.GetChild(5).ValueVec2, Weight);
    }
}