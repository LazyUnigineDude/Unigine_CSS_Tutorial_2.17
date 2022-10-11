using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "f4adf9bc5e2189ebd909964ec16ca955e3b86ab0")]
public class Interactor : Component
{
    private WorldIntersection Ray;
    public Node Camera;
    private Node Hand;

    private void Init()
    {
        Ray = new WorldIntersection();
        // Hand Item Position
        Hand = node.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
    }

    private void Update()
    {
        Unigine.Object Obj = World.GetIntersection(Camera.WorldPosition, Camera.GetWorldDirection() * 100, 0x00000002, Ray);
        if (Obj)
        {
            Visualizer.RenderPoint3D(Ray.Point, 0.1f, vec4.RED);

            if (Input.IsKeyDown(Input.KEY.E))
            {
                Hand.AddChild(Obj);
                Obj.Position = vec3.ZERO;

                // Change State to Equipped
                AnimationController _Shooter = GetComponent<AnimationController>(node.GetChild(0).GetChild(0));
                _Shooter.ChangeStateToEquipped();
                GunHandler _gun = GetComponent<GunHandler>(node);
                _gun.GetGun(Obj);
            }
        }
    }
}