using System.Collections.Generic;
using Unigine;

namespace UnigineApp.data.Scripts.AI
{
    internal class PathMaker
    {
        private const int RENDER_SEGMENTS = 50;
        private int TimeBetweenPoints;
        private List<dvec3> PathPoints;

        SplineGraph Path;
        float Weight = 0.0f;
        int num = 0;

        public PathMaker(int TimeBetweenPoints, List<dvec3> PathPoints) 
        {
            this.TimeBetweenPoints = TimeBetweenPoints;
            this.PathPoints = PathPoints;
            InitPath();
        }

        public dvec3 GetCurrentPathPosition() => Path.CalcSegmentPoint(num, Weight);

        public void MoveAlongPath()
        {
            Weight = MathLib.Clamp(Weight += (Game.IFps / TimeBetweenPoints), 0.0f, 1.0f);
            if (Weight == 1.0f) { Weight = 0; num++; }
            num %= Path.NumPoints;
        }

        
        public void MoveObject(Node Obj2Move)
        {
            dvec3 Point = Path.CalcSegmentPoint(num, Weight);
            vec3 Direc = Path.CalcSegmentTangent(num, Weight),
                  UpVec = Path.CalcSegmentUpVector(num, Weight);

            Obj2Move.WorldPosition = Point;
            Obj2Move.SetWorldDirection(Direc, UpVec, MathLib.AXIS.Y);
        }

        public void RenderPath()
        {
            for (int i = 0; i < Path.NumPoints; i++)
            {
                Visualizer.RenderPoint3D(Path.GetPoint(i), 0.1f, vec4.BLACK);

                dvec3 SPoint = Path.GetSegmentStartPoint(i),
                     STang = Path.GetSegmentStartTangent(i),
                     EPoint = Path.GetSegmentEndPoint(i),
                     ETang = Path.GetSegmentEndTangent(i);

                Visualizer.RenderVector(SPoint, SPoint + STang, vec4.GREEN);
                Visualizer.RenderVector(EPoint, EPoint + ETang, vec4.RED);

                for (int j = 0; j < RENDER_SEGMENTS; j++)
                {
                    dvec3 p0 = Path.CalcSegmentPoint(i, j / RENDER_SEGMENTS),
                         p1 = Path.CalcSegmentPoint(i, (j + 1) / RENDER_SEGMENTS);
                    Visualizer.RenderLine3D(p0, p1, vec4.WHITE);
                }
            }
        }

        void RotateTowards(dvec3 TowardsObject, Node Obj2Move, float Speed)
        {
            vec3 Vec1 = Obj2Move.GetWorldDirection(MathLib.AXIS.Y),
                 Vec2 = (vec3)(TowardsObject - Obj2Move.WorldPosition).Normalized;
            float Angle = MathLib.Angle(Vec1, Vec2, vec3.UP);
            Obj2Move.Rotate(-Obj2Move.GetWorldRotation().x, -Obj2Move.GetWorldRotation().y, Angle * Speed);
        }

        void MoveTowards(dvec3 TowardsObject, Node Obj2Move, int Speed)
        {
            dvec3 Pos = MathLib.Lerp(Obj2Move.WorldPosition,
                                    TowardsObject,
                                    Game.IFps * Speed / MathLib.Distance(Obj2Move.WorldPosition,
                                                                TowardsObject));
            Obj2Move.WorldPosition = Pos;
        }

        private void InitPath()
        {
            Path = new();
            PathPoints.ForEach(Obj => Path.AddPoint(Obj));

            for (int i = 0; i < PathPoints.Count; i++)
            {

                int num = i % Path.NumPoints, num2 = (i + 1) % Path.NumPoints;

                Path.AddSegment(
                    num,
                    new vec3(PathPoints[num2] - PathPoints[num]),
                    vec3.UP,
                    num2,
                    new vec3(PathPoints[num] - PathPoints[num2]),
                    vec3.UP);
            }
        }
    }
}