using System;
using System.Collections.Generic;
using Unigine;
using UnigineApp.data.Scripts.AI;

[Component(PropertyGuid = "98106011dd339a3c175db06bfaee8ace93f813d5")]
public class ShooterAI : Component
{
    public struct PathLists { public List<Node> List; }

    public Node MainCharacter;
    public AssetLinkNode BulletPrefab;
    public Node PhysicalTriggerNode;
    public Node NavigationNode;

    public List<Node> Obstacles;
    public List<PathLists> Paths;

    bool isVisible = false; 
    int CurrentHealth;
    private double DistanceRatio, CurrentTime;
    private float Weight, ViewDistance = 30;
    
    PathMaker MainPath;
    HealthBar Health;
    NavigationMaker NaviMesh;
    AIDetector Detection;
    AIShoot Shooter;

    enum AISTATE { IDLE, ALERT, SEARCH, AGGRESSIVE, SHOOT, DODGE }
    
    AISTATE STATE;

    private void Init()
    {
        // write here code to be called on component initialization
        Weight = 0;
        STATE = AISTATE.IDLE;

        Health = node.GetComponent<HealthBar>();
        CurrentHealth = Health.ShowHealth();

        FrustumSettings settings = new();
        settings.FOV = 40;
        settings.Aspect_Ratio = 1.5f;
        settings.zNear = 0.05f;
        settings.zFar = ViewDistance;

        Detection = new AIDetector
            (
                settings,
                node,
                MainCharacter,
                node.GetChild(0),
                PhysicalTriggerNode,
                4
            );

        Shooter = new AIShoot
            (
                1,
                BulletPrefab,
                MainCharacter,
                node.GetChild(0)
            );
    }

    private void Update()
    {
        // write here code to be called before updating each render frame

        Detection.CalculateView();

        if (Detection.TargetInsideView( 1, MainCharacter.Name))
        {
          double distance = MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition);
                 DistanceRatio = distance / ViewDistance;
        }
        //  AiSTATE();
    }

    void AiSTATE()
    {
        //if (CurrentHealth != Health.ShowHealth()) { Weight = 1; STATE = AISTATE.AGGRESSIVE; CurrentHealth = Health.ShowHealth(); }

        //switch (STATE)
        //{
        //    case AISTATE.IDLE:
        //        //Log.Message("IDLE\n");
        //        Weight = MathLib.Clamp(Weight -= Game.IFps, 0f, 1f);
        //        if (isVisible) STATE = AISTATE.ALERT;
        //        if (MathLib.Distance(node.WorldPosition, Path.GetCurrentPathPosition()) > 0.1f)
        //        {
        //          //  MoveTowards(Path.GetCurrentPathPosition(), node, 1);
        //         //   RotateTowards(Path.GetCurrentPathPosition(), node, 0.05f);
        //        }
        //        else
        //        {
        //            Path.MoveAlongPath();
        //            Path.MoveObject(node);
        //        }
        //        break;
        //    case AISTATE.ALERT:
        //        //Log.Message("ALRT\n");
        //        Weight = MathLib.Clamp(Weight += Game.IFps / (float)DistanceRatio, 0f, 1f);
        //        if (!isVisible) STATE = AISTATE.IDLE;
        //        if (Weight == 1f) STATE = AISTATE.AGGRESSIVE;

        //        RotateTowards(MainCharacter.WorldPosition, node, 0.005f);
        //        break;
        //    case AISTATE.SEARCH:
        //        //Log.Message("SRCH\n");
        //        Weight = MathLib.Clamp(Weight -= Game.IFps / 5, 0f, 1f);
        //        if (Weight == 0f) STATE = AISTATE.IDLE;
        //        if (isVisible) { STATE = AISTATE.AGGRESSIVE; Weight = 1; }
        //        MoveTowards(MainCharacter.WorldPosition, node, 3);
        //        RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
        //        break;
        //    case AISTATE.AGGRESSIVE:
        //        //Log.Message("AGRO\n");
        //        if (!isVisible) STATE = AISTATE.SEARCH;
        //        MoveTowards(MainCharacter.WorldPosition, node, 5);
        //        RotateTowards(MainCharacter.WorldPosition, node, 0.05f);
        //        if (MathLib.Distance(node.WorldPosition, MainCharacter.WorldPosition) < 20.0f) { STATE = AISTATE.SHOOT; CurrentTime = Game.Time; }
        //        break;
        //    case AISTATE.SHOOT:
        //        if (CurrentTime + 1 < Game.Time) { Shoot(); STATE = AISTATE.AGGRESSIVE; }
        //        RotateTowards(MainCharacter.WorldPosition, node, 0.01f);
        //        Visualizer.RenderLine3D(node.WorldPosition, node.WorldPosition + node.GetDirection(MathLib.AXIS.Y) * 50, vec4.BLUE);
        //        break;
        //    default:
        //        break;
        //}
    }

    void Shoot()
    {
        Shooter.CalculatePositions();
        Shooter.VisualizePrediction(2);

        double Speed = GetComponent<PhysicsController>(MainCharacter).getSpeed();
        Shooter.Shoot(Speed);
    }

}