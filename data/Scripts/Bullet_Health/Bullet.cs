using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "6cd6c2574f447bcaa2f5b8a17232798c0418e77b")]
public class Bullet : Component
{
    float StartTime;

    public float LifeTime = 5f;
    public int DamageAmount { get; set; }

    Body Rigid;

    void OnEnter(Body Body, int num)
    {
        node.DeleteLater();
        Body Body1 = Body.GetContactBody0(num),
             Body2 = Body.GetContactBody1(num),
             CapturedBody = null;

        if (Body1 && Body1 != Rigid) { CapturedBody = Body1; }
        else if (Body2 && Body2 != Rigid) { CapturedBody = Body2; }
        
        if (CapturedBody)
        {
            // WE hit a body
           // Log.Message($"{CapturedBody.Object.Name} {num}\n");
            HealthBar Health = GetComponent<HealthBar>(CapturedBody.Object);
            if (Health) { Health.DropHealth(DamageAmount); Log.Message($"{Health.ShowHealth()}\n"); }
           // Rigid.RemoveContactEnterCallback(OnEnter);
        }

        else
        {
           // Log.Message($"{Body.GetContactObject(num).Name} {num}\n");
            // We hit a collision
            HealthBar Health = GetComponent<HealthBar>(Body.GetContactObject(num));
            if (Health) { Health.DropHealth(DamageAmount); Log.Message($"{Health.ShowHealth()}\n"); }
        }
    }

    private void Init()
    {
        StartTime = Game.Time;
        Rigid = node.ObjectBodyRigid;
        Rigid.AddContactEnterCallback(OnEnter);
    }

    private void Update()
    {
        // write here code to be called before updating each render frame
        if (Game.Time - StartTime > LifeTime)
        {
            node.DeleteLater();
        }
    }
}