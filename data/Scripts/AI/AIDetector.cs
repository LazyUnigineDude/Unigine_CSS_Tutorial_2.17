using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;

namespace UnigineApp.data.Scripts.AI
{
    internal class FrustumSettings
    {
        public float 
            FOV,
            Aspect_Ratio,
            zNear, 
            zFar;
    }

    internal class AIDetector
    {
        bool isDodging = false;
        int ENEMY_MASK, DodgeDistance = 0;

        BoundFrustum BF;
        quat Horizontal_Reset = new quat(90, 0, 0), Rotation;
        mat4 View, Frustum;
        vec3 Position;

        FrustumSettings Settings;
        Node MainObj, Target, DetectionPosition;
        PhysicalTrigger DodgeArea;

        public AIDetector
            (
                FrustumSettings Settings,
                Node MainObj,
                Node Target,
                Node DetectionPosition,
                Node PhysicalTriggerNode,
                int ENEMY_MASK
            ) 
        {
            this.Settings = Settings;
            this.MainObj = MainObj;
            this.Target = Target;
            this.DetectionPosition = DetectionPosition;
            this.ENEMY_MASK = ENEMY_MASK;

            DodgeArea = PhysicalTriggerNode as PhysicalTrigger;
            DodgeArea.AddEnterCallback(EnterDodgeArea);
        }

        public void Dodge(bool isDodging, int DodgeDistance) 
        { 
            this.isDodging = isDodging;
            this.DodgeDistance = DodgeDistance;
        }

        void EnterDodgeArea(Body Body)
        {
            if (Body.Object.Name != MainObj.Name)
            {
                Unigine.Object ObjectCaught = World.GetIntersection(
                    Body.Position, 
                    Body.Position + Body.Object.BodyLinearVelocity, 
                    ENEMY_MASK);

                if (
                    ObjectCaught &&
                    ObjectCaught.Name == MainObj.Name && 
                    isDodging
                    )
                {
                    vec3 MainVector = Body.Object.BodyLinearVelocity.Normalized;
                    float Angle = MathLib.Angle(
                        new vec3(MainObj.WorldPosition) + MainObj.GetWorldDirection(MathLib.AXIS.Y), 
                        MainVector,
                        vec3.UP
                        );

                    Angle = (Angle > 0) ? 90 : -90;

                    vec3 RotatingVector = new vec3(
                        MainVector.x * Math.Cos(Angle) - MainVector.y * Math.Sin(Angle),
                        MainVector.x * Math.Sin(Angle) + MainVector.y * Math.Cos(Angle),
                        0);

                    MainObj.WorldPosition += RotatingVector * DodgeDistance;
                }
            }
        }

        public void RenderView()
        {
            Visualizer.RenderSphere(DodgeArea.Size.x, MainObj.WorldTransform, vec4.RED);
            Visualizer.RenderFrustum(Frustum, View, vec4.BLACK);
        }

        public void CalculateView() 
        {
            Frustum = MathLib.Perspective(
                Settings.FOV,
                Settings.Aspect_Ratio,
                Settings.zNear,
                Settings.zFar);
            Rotation = MainObj.GetWorldRotation() * Horizontal_Reset;
            Position = (vec3)DetectionPosition.WorldPosition;
            View.Set(Rotation, Position);
            BF.Set(Frustum, MathLib.Inverse(View));
        }

        public bool TargetInsideView(int MASK, string TargetName)
        {
            if (BF.Inside((vec3)Target.WorldPosition))
            {
                Unigine.Object DetectFirstObjectCaught = World.GetIntersection(
                    DetectionPosition.WorldPosition,
                    Target.WorldPosition,
                    MASK);

                if (DetectFirstObjectCaught && 
                    DetectFirstObjectCaught.Name == TargetName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
