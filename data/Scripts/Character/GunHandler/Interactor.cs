using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "f4adf9bc5e2189ebd909964ec16ca955e3b86ab0")]
public class Interactor : Component
{
    [ShowInEditor, ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
    private int Mask;
    private WorldIntersection Ray;

    private ivec2 Item;
    private Node CaughtItem;
    private Node Camera;

    Gui GUI;
    WidgetLabel Label;
    DatabaseController Database;

    public void Initialize(Player _Camera) 
    { 
        Ray = new();
        Camera = _Camera;
        GUI = Gui.GetCurrent(); 

        Label = new();
        Label.FontOutline = 1;
        Label.FontSize = 21;
        GUI.AddChild(Label, Gui.ALIGN_CENTER | Gui.ALIGN_OVERLAP);
        Label.SetPosition(Label.PositionX - 50, Label.PositionY - 50);
        Database = GetComponent<DatabaseController>(World.GetNodeByID(DatabaseController.NODE_ID));
    }

    public bool DetectItem() 
    {
        Unigine.Object Obj = World.GetIntersection(
            Camera.WorldPosition,
            Camera.WorldPosition + Camera.GetWorldDirection() * 5, 
            Mask,
            Ray
            );

        if (Obj) 
        {
            if(CaughtItem != Obj)
            {
                Item = new ivec2(
                       Obj.GetProperty(0).ParameterPtr.GetChild(0).ValueInt,
                       Obj.GetProperty(0).ParameterPtr.GetChild(1).ValueInt);
                CaughtItem = Obj;
                DatabaseController.ITEM_TYPE Type = Database.GetType(Item.x);
                string Text = " ";
                switch (Type)
                {
                    case DatabaseController.ITEM_TYPE.DEFAULT:
                        Text += "Name: " + Database.GetName(Item.x) +
                             "\nAmount: " + Item.y;
                        break;
                    case DatabaseController.ITEM_TYPE.GUN:
                        Text += "Name: " + Database.GetName(Item.x) +
                             "\nAmmo: " + Item.y;
                        break;
                    case DatabaseController.ITEM_TYPE.TRIGGER:
                        Text += "Interact";
                        break;
                    default:
                        break;
                }

                Label.Text = Text;
            }
            return true; 
        }
        Label.Text = " ";
        CaughtItem = null;
        return false;
    }

    public ivec2 GetItem()
    {
        CaughtItem = null;
        if (DetectItem())
        {
            if(Database.GetType(Item.x) != DatabaseController.ITEM_TYPE.TRIGGER) { CaughtItem.DeleteLater(); }
            else
            {
                DoorOpener Door = GetComponent<DoorOpener>(CaughtItem.GetChild(0));
                if(Door != null) { Door.OpenDoor(); }
            }
            return Item;
        }
        return ivec2.ZERO;
    }

    public void ShutDown()
    {
        GUI = Gui.GetCurrent();
        if (GUI.IsChild(Label) == 1) { GUI.RemoveChild(Label); }
        Label.DeleteLater();
    }
}