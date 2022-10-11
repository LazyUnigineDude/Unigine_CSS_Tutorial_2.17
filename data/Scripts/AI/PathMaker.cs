using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "2d065d592ca426e4b487c4f3bb87c5b629e0819d")]
public class PathMaker : Component
{
    public int TimeBetweenPoints = 5;
    public List<Node> PathPoints;

    SplineGraph Path;
    float Weight = 0.0f;
    int num = 0;

    public void InitPath()
    {

        Path = new();
        for (int i = 0; i < PathPoints.Count; i++) { Path.AddPoint(PathPoints[i].WorldPosition); }
        for (int i = 0; i < PathPoints.Count; i++)
        {
            int num = i % Path.NumPoints, num2 = (i + 1) % Path.NumPoints;

            Path.AddSegment(num, PathPoints[num].GetWorldDirection(MathLib.AXIS.Y), PathPoints[num].GetWorldDirection(MathLib.AXIS.Z),
                            num2, PathPoints[num2].GetWorldDirection(MathLib.AXIS.NY), PathPoints[num2].GetWorldDirection(MathLib.AXIS.Z));
        }
    }

    public void MoveAlongPath()
    {

        Weight = MathLib.Clamp(Weight += (Game.IFps / TimeBetweenPoints), 0.0f, 1.0f);
        if (Weight == 1.0f) { Weight = 0; num++; }
        num %= Path.NumPoints;
    }

    public dvec3 GetCurrentPathPosition()
    {
        return Path.CalcSegmentPoint(num, Weight);
    }

    public void MoveObject(Node Obj2Move)
    {

        dvec3 Point = Path.CalcSegmentPoint(num, Weight);
        vec3  Direc = Path.CalcSegmentTangent(num, Weight),
              UpVec = Path.CalcSegmentUpVector(num, Weight);

        Obj2Move.WorldPosition = Point;
        Obj2Move.SetWorldDirection(Direc, UpVec, MathLib.AXIS.Y);
    }

    public void RenderPath()
    {

        int segments = 50;
        for (int i = 0; i < Path.NumPoints; i++)
        {
            Visualizer.RenderPoint3D(Path.GetPoint(i), 0.1f, vec4.BLACK);

            dvec3 SPoint = Path.GetSegmentStartPoint(i),
                 STang = Path.GetSegmentStartTangent(i),
                 EPoint = Path.GetSegmentEndPoint(i),
                 ETang = Path.GetSegmentEndTangent(i);

            Visualizer.RenderVector(SPoint, SPoint + STang, vec4.GREEN);
            Visualizer.RenderVector(EPoint, EPoint + ETang, vec4.RED);

            for (int j = 0; j < segments; j++)
            {
                dvec3 p0 = Path.CalcSegmentPoint(i, j / segments),
                     p1 = Path.CalcSegmentPoint(i, (j + 1) / segments);
                Visualizer.RenderLine3D(p0, p1, vec4.WHITE);
            }
        }
    }
}