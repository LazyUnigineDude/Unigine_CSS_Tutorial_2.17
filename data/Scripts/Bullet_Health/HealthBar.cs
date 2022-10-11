using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "14db8ba56855c8c05186a51f2616aff264524ea9")]
public class HealthBar : Component
{
    [ShowInEditor]
    private int Health;

    public int ShowHealth() { return Health; }
    public void DropHealth(int amount) { Health -= amount; Check(); }
    private void Check() { if (Health <= 0) node.DeleteLater(); }
}