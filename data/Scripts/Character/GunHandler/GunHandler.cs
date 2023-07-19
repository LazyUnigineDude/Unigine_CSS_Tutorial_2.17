using System.Reflection.Metadata.Ecma335;
using Unigine;
using static System.Net.Mime.MediaTypeNames;

[Component(PropertyGuid = "ac40d1b1498f1f5c0f2e330b6630ecee0e8fcf75")]
public class GunHandler : Component
{
    [ShowInEditor]
    private Node HandPosition;

    public bool isHolding;
    public bool isGrabbed;
    ivec2 GunValue;

    Node Gun;
    Player Camera;
    int AmountInGun = 0;
    float RoFTime = 0;
    DatabaseController Database;
    DatabaseController.GUNPROP GunProp;

    public ivec2 GUIGunValue() => new ivec2(AmountInGun, GunValue.y);
    public void Initialize(Player _Camera)
    {
        Database = GetComponent<DatabaseController>(World.GetNodeByID(DatabaseController.NODE_ID));
        Camera = _Camera;
    }
    
    public void GetGun(ivec2 _Gun) 
    {
        GunValue = _Gun;
        GunProp = Database.GetGunProperty(_Gun.x);
        isGrabbed = true;
    }

    public void Equip() 
    {
        Gun = World.LoadNode(Database.GetPrefabPath(GunValue.x));
        HandPosition.AddChild(Gun);
        Gun.Position = vec3.ZERO;
        Gun.SetRotation(quat.ZERO);

        isHolding = true;
        Reload();
    }

    public void UnEquip()
    {
        Gun.DeleteLater();
        GunValue.y += AmountInGun;
        isHolding = false;
    }

    public void Shoot(float Time)
    {
        if(isHolding && Time > RoFTime)
        {
            RoFTime = Time + (1 / GunProp.RoF);
            Shoot(Camera.WorldPosition + new dvec3(Camera.GetWorldDirection() * 100));
        }
    }

    void Shoot(dvec3 Lookat)
    {
        if (AmountInGun == 0) { Reload(); }
        else if (AmountInGun > 0)
        {
            Node _bullet = World.LoadNode(GunProp.BulletPath);
            _bullet.WorldPosition = Gun.GetChild(0).WorldPosition + Gun.GetWorldDirection(MathLib.AXIS.X);
            _bullet.WorldLookAt(Lookat);

            Bullet x = GetComponent<Bullet>(_bullet);
            x.DamageAmount = GunProp.Damage;

            _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * 100);
            AmountInGun--;
        }
    }

    public void Reload()
    {
        int reload = GunProp.Reload - AmountInGun;

        if (GunValue.y + AmountInGun == 0)
        {
            Log.Message("Empty\n");
        }
        else if (GunValue.y > reload)
        {
            GunValue.y -= reload;
            AmountInGun += reload;
        }
        else if (GunValue.y <= reload)
        {
            AmountInGun += GunValue.y;
            GunValue.y = 0;
        }
    }

    //private void Update()
    //{
    //    // write here code to be called before updating each render frame
    //    if (isHolding)
    //    {
    //        if (Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.LEFT))
    //        {
    //            if (Game.Time > RoFTime)
    //            {
    //                RoFTime = Game.Time + (1 / rateoffire);
    //                Shoot(Game.Player.WorldPosition + (Game.Player.GetWorldDirection() * 100));
    //            }
    //        }

    //        if (Input.IsKeyDown(Input.KEY.R))
    //        {
    //            Reload();
    //        }
    //    }
    //}

    //public void GetGun(ivec2 Item)
    //{
    //    this.Gun = Gun;
    //    isHolding = true;

    //    BulletPrefab = new AssetLinkNode(Gun.GetProperty(0).GetParameterPtr(0).ValueFile);
    //    damage = Gun.GetProperty(0).GetParameterPtr(1).ValueInt;
    //    rateoffire = Gun.GetProperty(0).GetParameterPtr(2).ValueFloat;
    //    CurrentBullets = Gun.GetProperty(0).GetParameterPtr(3).ValueInt;
    //    AmountPerShell = Gun.GetProperty(0).GetParameterPtr(4).ValueInt;

    //    GunHUD = GetComponent<HUDMaker>(HUD);
    //    Reload();
    //}

    //void Shoot(dvec3 Lookat)
    //{
    //    if (AmountInGun == 0) { Reload(); }
    //    else if (AmountInGun > 0)
    //    {
    //        Node _bullet = BulletPrefab.Load();
    //        _bullet.WorldPosition = Game.Player.WorldPosition + Game.Player.GetWorldDirection();
    //        _bullet.WorldLookAt(Lookat);

    //        Bullet x = GetComponent<Bullet>(_bullet);
    //        x.DamageAmount = damage;

    //        _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * 100);
    //        AmountInGun--;
    //        GunHUD.UpdateGun(AmountInGun, CurrentBullets);
    //    }
    //}

    //void Reload()
    //{

    //    int reload = AmountPerShell - AmountInGun;

    //    if (CurrentBullets == 0)
    //    {
    //        Log.Message("Empty\n");
    //    }
    //    else if (CurrentBullets > reload)
    //    {
    //        CurrentBullets -= reload;
    //        AmountInGun += reload;
    //    }
    //    else if (CurrentBullets <= reload)
    //    {
    //        AmountInGun += CurrentBullets;
    //        CurrentBullets = 0;
    //    }

    //    GunHUD.UpdateGun(AmountInGun, CurrentBullets);
    //}
    //void ShootRayCast()
    //{
    //	//Unigine.Object Obj1 = World.GetIntersection(Gun.GetChild(0).WorldPosition, MainCamera.GetWorldDirection() * 100, 0x00000001, Ray);
    //	Unigine.Object Obj2 = World.GetIntersection(MainCamera.WorldPosition, MainCamera.WorldPosition + (MainCamera.GetWorldDirection() * 100), 0x00000001, Ray2);

    //	if (Obj2) Shoot(Ray2.Point);
    //	else Shoot(MainCamera.WorldPosition + (MainCamera.GetWorldDirection() * 100));
    //	//Visualizer.RenderPoint3D(Ray.Point, 0.1f, vec4.BLACK,true,3); // Realistic
    //	Visualizer.RenderPoint3D(Ray2.Point, 0.1f, vec4.GREEN, true, 3);    // Accurate
    //}

    //void ShootShotGun()
    //{

    //	mat4 Frustum = MathLib.Perspective(40, 1.5f, 0.05f, 4);
    //	quat Rotation = node.GetWorldRotation() * new quat(90, 0, 0);
    //	mat4 View = new(Rotation, Gun.GetChild(0).WorldPosition);

    //	Visualizer.RenderFrustum(Frustum, View, vec4.BLACK);

    //	if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
    //	{
    //		Visualizer.RenderFrustum(Frustum, View, vec4.GREEN, 2);

    //		BoundFrustum bf = new(Frustum, MathLib.Inverse(View));

    //		List<Node> obj = new();

    //		World.GetIntersection(bf, Node.TYPE.OBJECT_MESH_STATIC, obj);

    //		foreach (var Objects in obj)
    //		{
    //			Log.Message("{0} \n", Objects.Name);
    //		}
    //	}
}