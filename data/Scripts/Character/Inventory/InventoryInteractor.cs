using Unigine;

namespace UnigineApp.data.Scripts.Database_Inventory
{
    internal class InventoryInteractor
    {
        int MaskNum;
        Player Camera;
        WidgetLabel Label;
        WorldIntersection WorldIntersection;
        Unigine.Object Item;
        ivec2 _item;
        Gui GUI;
        dvec3 CamViewDir;
        readonly DatabaseController Database;

        public InventoryInteractor() { }
        public InventoryInteractor(Player Camera, int MaskNum, DatabaseController database)
        {
            this.Camera = Camera;
            this.MaskNum = MaskNum;
            WorldIntersection = new();

            GUI = Gui.GetCurrent();
            Label = new WidgetLabel();
            Label.FontOutline = 1;
            Label.FontSize = 21;
            GUI.AddChild(Label, Gui.ALIGN_CENTER | Gui.ALIGN_OVERLAP);
            Label.SetPosition(Label.PositionX - 50, Label.PositionY - 50);
            Database = database;
        }

        public bool DetectItem()
        {
            CamViewDir = (dvec3)Camera.GetWorldDirection() * 5 + Camera.WorldPosition;

            Unigine.Object _Item = World.GetIntersection(
                Camera.WorldPosition,
                CamViewDir,
                MaskNum,
                WorldIntersection
                );

            if( _Item != null )
            {
                if(Item != _Item)
                {
                    _item = new ivec2(
                        _Item.GetProperty(0).ParameterPtr.GetChild(0).ValueInt,
                        _Item.GetProperty(0).ParameterPtr.GetChild(1).ValueInt);

                    Item = _Item;
                    Label.Text = "Name: " + Database.GetName(_item.x) +
                                 "\nAmount: " + _item.y;
                }
                return true;
            }

            Label.Text = " ";
            Item = null;
            return false;
        }

        public ivec2 GetItem()
        {
            Item = null;
            if (DetectItem())
            {
                Log.Message($"{_item.x}  {_item.y}\n");
                Item.DeleteLater();
                return _item;
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
}
