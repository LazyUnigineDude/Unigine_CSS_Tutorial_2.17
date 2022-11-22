using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "98106011dd339a3c175db06bfaee8ace93f813d5")]
public class ShooterAI : Component
{
    public Node PathMakerNode;
    public Node MainCharacter;
    private double DistanceRatio, CurrentTime;
    private float Weight, ViewDistance;
    public AssetLinkNode BulletPrefab;
    public Node PhysicalTriggerNode;

    BoundFrustum BF;
    bool isVisible = false;

    enum AISTATE { IDLE, ALERT, SEARCH, AGGRESSIVE, SHOOT, DODGE }
    AISTATE STATE;
    quat HorReset = new quat(90, 0, 0);
    PathMaker Path;
    mat4 View;
    HealthBar Health;
    int CurrentHealth;
    PhysicalTrigger DodgeArea;

    public Node NavNode;
    NavigationMesh NavMesh;
    dvec3[] PathPoints = new dvec3[4];
    PathRoute[] Pathing = new PathRoute[4];

    private void Init()
    {
        // write here code to be called on component initialization

        ViewDistance = 30;
        Weight = 0;
        STATE = AISTATE.DODGE;
        Path = GetComponent<PathMaker>(PathMakerNode);
        BF = new();
        View = new();
        Path.InitPath();

        Health = node.GetComponent<HealthBar>();
        CurrentHealth = Health.ShowHealth();
        DodgeArea = PhysicalTriggerNode as PhysicalTrigger;
        DodgeArea.AddEnterCallback(EnterDodgeArea);
        NavCreate();
    }

    void NavCreate()
    {
        NavMesh = NavNode as NavigationMesh;

        for (int i = 0; i < PathPoints.Length; i++)
        {
            PathPoints[i] = NavNode.GetChild(i).WorldPosition;
        }

        for (int i = 0; i < Pathing.Length; i++)
        {
            Pathing[i] = new PathRoute();
            Pathing[i].Create2D(PathPoints[i], PathPoints[(i + 1) % PathPoints.Length]);
        }

    }

    void NavVisualize()
    {
        NavMesh.RenderVisualizer();

        foreach (var Path in Pathing)
        {
            if (Path.IsReached)
            {
                Path.RenderVisualizer(vec4.WHITE);
            }
        }
    }

    private void EnterDodgeArea(Body Body)
    {
        if (Body.Object.Name != node.Name)
        {
            Log.Message($"{Body.Object.Name} Entered\n");
            Unigine.Object MainObj = World.GetIntersection(Body.Position, Body.Position + Body.Object.BodyLinearVelocity, 4);
            Visualizer.RenderLine3D(Body.Position, Body.Position + Body.Object.BodyLinearVelocity, vec4.YELLOW, 1);

            if (MainObj && MainObj.Name == node.Name && STATE == AISTATE.DODGE) {

                vec3 MainVector = Body.Object.BodyLinearVelocity.Normalized;
                float Angle = MathLib.Angle(new vec3(node.WorldPosition) + node.GetWorldDirection(MathLib.AXIS.Y), MainVector, vec3.UP);
                Angle = (Angle > 0) ? 90 : -90;
                vec3 RotatingVector = new vec3(
                    MainVector.x * Math.Cos(Angle) - MainVector.y * Math.Sin(Angle),
                    MainVector.x * Math.Sin(Angle) + MainVector.y * Math.Cos(Angle),
                    0);
                Visualizer.RenderVector(Body.Position, Body.Position + MainVector, vec4.BLUE, 0.25f, true, 1);
                Visualizer.RenderVector(Body.Position, Body.Position + RotatingVector, vec4.BLUE, 0.25f, true, 1);
                node.WorldPosition += RotatingVector * 5;
            }
        }
    }

    private void Update()
    {
        // write here code to be called before updating each render frame

        mat4 Frustum = MathLib.Perspective(40, 1.5f, 0.05f, ViewDistance);
        quat Rotation = node.GetWorldRotation() * HorReset;
        vec3 Pos = (vec3)node.GetChild(0).WorldPosition;
        View.Set(Rotation, Pos);

       // Visualizer.RenderFrustum(Frustum, View, vec4.BLACK);
        BF.Set(Frustum, MathLib.Inverse(View));

        if (BF.Inside((vec3)MainCharacter.WorldPosition))
        {
            Unigine.Object x = World.GetIntersection(node.GetChild(0).WorldPosition, MainCharacter.WorldPosition, 1);
            Visualizer.RenderLine3D(node.GetChild(0).WorldPosition, MainCharacter.WorldPosition, vec4.RED);

            if (x && x.Name == MainCharacter.GetChild(0).Name)
            {
                isVisible = true;
                double distance = MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition);
                DistanceRatio = distance / ViewDistance;
            }
            else isVisible = false;
        }
       // Path.RenderPath();
      //  AiSTATE();
        NavVisualize();
    }

    void AiSTATE()
    {
        if (CurrentHealth != Health.ShowHealth()) { Weight = 1; STATE = AISTATE.AGGRESSIVE; CurrentHealth = Health.ShowHealth(); }

        switch (STATE)
        {
            case AISTATE.IDLE:
                //Log.Message("IDLE\n");
                Weight = MathLib.Clamp(Weight -= Game.IFps, 0f, 1f);
                if (isVisible) STATE = AISTATE.ALERT;
                if (MathLib.Distance(node.WorldPosition, Path.GetCurrentPathPosition()) > 0.1f)
                {
                    MoveTowards(Path.GetCurrentPathPosition(), node, 1);
                    RotateTowards(Path.GetCurrentPathPosition(), node, 0.05f);
                }
                else
                {
                    Path.MoveAlongPath();
                    Path.MoveObject(node);
                }
                break;
            case AISTATE.ALERT:
                //Log.Message("ALRT\n");
                Weight = MathLib.Clamp(Weight += Game.IFps / (float)DistanceRatio, 0f, 1f);
                if (!isVisible) STATE = AISTATE.IDLE;
                if (Weight == 1f) STATE = AISTATE.AGGRESSIVE;

                RotateTowards(MainCharacter.WorldPosition, node, 0.005f);
                break;
            case AISTATE.SEARCH:
                //Log.Message("SRCH\n");
                Weight = MathLib.Clamp(Weight -= Game.IFps / 5, 0f, 1f);
                if (Weight == 0f) STATE = AISTATE.IDLE;
                if (isVisible) { STATE = AISTATE.AGGRESSIVE; Weight = 1; }
                MoveTowards(MainCharacter.WorldPosition, node, 3);
                RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
                break;
            case AISTATE.AGGRESSIVE:
                //Log.Message("AGRO\n");
                if (!isVisible) STATE = AISTATE.SEARCH;
                MoveTowards(MainCharacter.WorldPosition, node, 5);
                RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
                if (MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition) < 20.0f) { STATE = AISTATE.SHOOT; CurrentTime = Game.Time; }
                break;
            case AISTATE.SHOOT:
                if (CurrentTime + 1 < Game.Time) { Shoot(); STATE = AISTATE.AGGRESSIVE; }
                RotateTowards(MainCharacter.WorldPosition, node, 0.01f);
                Visualizer.RenderLine3D(node.WorldPosition, node.WorldPosition + node.GetDirection(MathLib.AXIS.Y) * 50, vec4.BLUE);
                break;
            default:
                break;
        }
    }

    void Shoot()
    {

        dvec3 futurePoint = MainCharacter.WorldPosition + MainCharacter.BodyLinearVelocity.Normalized;
        futurePoint.z = 1;
        dmat4 _dmat4 = new dmat4(MainCharacter.GetWorldRotation(),futurePoint);
        Visualizer.RenderCapsule(0.5f, 1.4f, _dmat4, vec4.BLACK,2);
        Visualizer.RenderPoint3D(futurePoint, 0.05f, vec4.YELLOW,false, 2);

        double FutureDistance = MathLib.Distance(futurePoint, MainCharacter.WorldPosition),
               Distance = MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition),
               Speed = GetComponent<PhysicsController>(MainCharacter).getSpeed();


        Node _bullet = BulletPrefab.Load();
        _bullet.WorldPosition = node.GetChild(0).WorldPosition;

        Bullet x = GetComponent<Bullet>(_bullet);
        x.DamageAmount = 1;

        if (Speed <= 1 && Speed > 0)  {
            _bullet.WorldLookAt(MathLib.Lerp(MainCharacter.WorldPosition, futurePoint, (float)Speed / FutureDistance));
            _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance);
        }
        else if (Speed > 1)  {
            _bullet.WorldLookAt(futurePoint);
            _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance * (float)(Speed / FutureDistance));
        }
        else {
            _bullet.WorldLookAt(node.GetChild(0).WorldPosition + node.GetChild(0).GetWorldDirection(MathLib.AXIS.Y));
            _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * (float)Distance);
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
}