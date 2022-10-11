using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ac40d1b1498f1f5c0f2e330b6630ecee0e8fcf75")]
public class GunHandler : Component
{
    public Node HUD;

    Unigine.Object Gun;
    bool isHolding = false;
    AssetLinkNode BulletPrefab;
    int damage, CurrentBullets, AmountPerShell, AmountInGun = 0;
    float RoFTime, rateoffire;

    HUDMaker GunHUD;

    private void Update()
    {
        // write here code to be called before updating each render frame
        if (isHolding)
        {
            if (Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.LEFT))
            {
                if (Game.Time > RoFTime)
                {
                    RoFTime = Game.Time + (1 / rateoffire);
                    Shoot(Game.Player.WorldPosition + (Game.Player.GetWorldDirection() * 100));
                }
            }

            if (Input.IsKeyDown(Input.KEY.R))
            {
                Reload();
            }
        }
    }

    public void GetGun(Unigine.Object Gun)
    {
        this.Gun = Gun;
        isHolding = true;

        BulletPrefab = new AssetLinkNode(Gun.GetProperty(0).GetParameterPtr(0).ValueFile);
        damage = Gun.GetProperty(0).GetParameterPtr(1).ValueInt;
        rateoffire = Gun.GetProperty(0).GetParameterPtr(2).ValueFloat;
        CurrentBullets = Gun.GetProperty(0).GetParameterPtr(3).ValueInt;
        AmountPerShell = Gun.GetProperty(0).GetParameterPtr(4).ValueInt;

        GunHUD = GetComponent<HUDMaker>(HUD);
        Reload();
    }

    void Shoot(dvec3 Lookat)
    {
        if (AmountInGun == 0) { Reload(); }
        else if (AmountInGun > 0)
        {
            Node _bullet = BulletPrefab.Load();
            _bullet.WorldPosition = Gun.GetChild(0).WorldPosition;
            _bullet.WorldLookAt(Lookat);

            Bullet x = GetComponent<Bullet>(_bullet);
            x.DamageAmount = damage;

            _bullet.ObjectBodyRigid.AddLinearImpulse(_bullet.GetWorldDirection(MathLib.AXIS.Y) * 100);
            AmountInGun--;
            GunHUD.UpdateGun(AmountInGun, CurrentBullets);
        }
    }

    void Reload()
    {

        int reload = AmountPerShell - AmountInGun;

        if (CurrentBullets == 0)
        {
            Log.Message("Empty\n");
        }
        else if (CurrentBullets > reload)
        {
            CurrentBullets -= reload;
            AmountInGun += reload;
        }
        else if (CurrentBullets < reload)
        {
            AmountInGun += CurrentBullets;
            CurrentBullets = 0;
        }

        GunHUD.UpdateGun(AmountInGun, CurrentBullets);
    }
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