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

    BoundFrustum BF;
    bool isInsideFrustum = false;

    enum AISTATE { IDLE, ALERT, SEARCH, AGGRESSIVE, SHOOT }
    AISTATE STATE;
    quat HorReset = new quat(90, 0, 0);
    PathMaker Path;
    mat4 View;
    HealthBar Health;
    int CurrentHealth;

    private void Init()
    {
        // write here code to be called on component initialization

        ViewDistance = 30;
        Weight = 0;
        STATE = AISTATE.IDLE;
        Path = GetComponent<PathMaker>(PathMakerNode);
        BF = new();
        View = new();
        Path.InitPath();

        Health = node.GetComponent<HealthBar>();
        CurrentHealth = Health.ShowHealth();
    }

    private void Update()
    {
        // write here code to be called before updating each render frame

        mat4 Frustum = MathLib.Perspective(40, 1.5f, 0.05f, ViewDistance);
        quat Rotation = node.GetWorldRotation() * HorReset;
        vec3 Pos = (vec3)node.GetChild(0).WorldPosition;
        View.Set(Rotation, Pos);

        Visualizer.RenderFrustum(Frustum, View, vec4.BLACK);
        BF.Set(Frustum, MathLib.Inverse(View));

        if (BF.Inside((vec3)MainCharacter.WorldPosition))
        {
            isInsideFrustum = true;
            double distance = MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition);
            DistanceRatio = distance / ViewDistance;
        }
        else isInsideFrustum = false;

        Path.RenderPath();
        AiSTATE();
    }

    void AiSTATE()
    {

        if (CurrentHealth != Health.ShowHealth()) { Weight = 1; STATE = AISTATE.AGGRESSIVE; CurrentHealth = Health.ShowHealth(); }

        switch (STATE)
        {
            case AISTATE.IDLE:
                //Log.Message("IDLE\n");
                Weight = MathLib.Clamp(Weight -= Game.IFps, 0f, 1f);
                if (isInsideFrustum) STATE = AISTATE.ALERT;
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
                if (!isInsideFrustum) STATE = AISTATE.IDLE;
                if (Weight == 1f) STATE = AISTATE.AGGRESSIVE;

                RotateTowards(MainCharacter.WorldPosition, node, 0.005f);
                break;
            case AISTATE.SEARCH:
                //Log.Message("SRCH\n");
                Weight = MathLib.Clamp(Weight -= Game.IFps / 5, 0f, 1f);
                if (Weight == 0f) STATE = AISTATE.IDLE;
                if (isInsideFrustum) { STATE = AISTATE.AGGRESSIVE; Weight = 1; }
                MoveTowards(MainCharacter.WorldPosition, node, 3);
                RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
                break;
            case AISTATE.AGGRESSIVE:
                //Log.Message("AGRO\n");
                if (!isInsideFrustum) STATE = AISTATE.SEARCH;
                MoveTowards(MainCharacter.WorldPosition, node, 5);
                RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
                if (MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition) < 20.0f) { STATE = AISTATE.SHOOT; CurrentTime = Game.Time; }
                break;
            case AISTATE.SHOOT:
                if (CurrentTime + 1 < Game.Time) { Shoot(); STATE = AISTATE.AGGRESSIVE; }
                RotateTowards(MainCharacter.WorldPosition, node, 0.01f);
                break;
            default:
                break;
        }
    }

    void Shoot()
    {

        Node _bullet = BulletPrefab.Load();
        _bullet.WorldPosition = node.GetChild(0).WorldPosition;
        _bullet.WorldLookAt(node.GetChild(0).WorldPosition + node.GetChild(0).GetWorldDirection(MathLib.AXIS.Y));

        Bullet x = GetComponent<Bullet>(_bullet);
        x.DamageAmount = 1;

        _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * 50);
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