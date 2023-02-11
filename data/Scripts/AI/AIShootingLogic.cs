using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;

namespace UnigineApp.data.Scripts.AI
{
    internal class AIShootingLogic
    {
        dvec3 FuturePoint;
        Node Target, GunPoint;
        AssetLinkNode BulletPrefab;

        double FutureDistance, Distance;
        int Damage;

        public AIShootingLogic(
               int Damage,
               AssetLinkNode BulletPrefab,
               Node Target,
               Node GunPoint
            )
        {
            this.Damage = Damage;
            this.BulletPrefab = BulletPrefab;
            this.Target = Target;
            this.GunPoint = GunPoint;
        }

        public void CalculatePositions()
        {
            FuturePoint = Target.WorldPosition + Target.BodyLinearVelocity.Normalized;
            FuturePoint.z = 1;

            FutureDistance = MathLib.Distance(FuturePoint, Target.WorldPosition);
            Distance = MathLib.Distance(GunPoint.WorldPosition, Target.WorldPosition);
        }

        public void VisualizePrediction(int Seconds)
        {
            dmat4 _dmat4 = new dmat4(Target.GetWorldRotation(), FuturePoint);

            Visualizer.RenderCapsule(0.5f, 1.4f, _dmat4, vec4.BLACK, Seconds);
            Visualizer.RenderPoint3D(FuturePoint, 0.05f, vec4.YELLOW, false, Seconds);
        }

        public void Shoot(double Speed)
        {
            Node _Bullet = BulletPrefab.Load();
            _Bullet.WorldPosition = GunPoint.WorldPosition;

            Bullet _B = _Bullet.GetComponent<Bullet>();
            _B.DamageAmount = Damage;

            if (Speed <= 1 && Speed > 0)
            {
                _Bullet.WorldLookAt(MathLib.Lerp(Target.WorldPosition, FuturePoint, (float)Speed / FutureDistance));
                _Bullet.ObjectBodyRigid.AddLinearImpulse(_Bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance);
            }
            else if (Speed > 1)
            {
                _Bullet.WorldLookAt(FuturePoint);
                _Bullet.ObjectBodyRigid.AddLinearImpulse(_Bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance * (float)(Speed / FutureDistance));
            }
            else
            {
                _Bullet.WorldLookAt(GunPoint.GetChild(0).WorldPosition + GunPoint.GetChild(0).GetWorldDirection(MathLib.AXIS.Y));
                _Bullet.ObjectBodyRigid.AddLinearImpulse(_Bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance);
            }
        }
    }
}
