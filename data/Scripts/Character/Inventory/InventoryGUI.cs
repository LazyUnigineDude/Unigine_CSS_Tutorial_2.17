using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using Unigine;

namespace UnigineApp.data.Scripts.Database_Inventory
{
    internal class InventoryGUI
    {
        private InventoryMaker Inventory;
        WidgetCanvas BackGround;
        WidgetSprite ThrowAway;
        List<Widget> ImageList, TextList;
        WidgetGridBox ImageGrid, TextGrid;
        Dictionary<Widget, int> Map;
        private readonly DatabaseController Database;

        public InventoryGUI(InventoryMaker inventory, DatabaseController _database)
        {
            Inventory = inventory;
            Database = _database;
            CreateBackground();
        }

        public void Hide()
        {
            Gui GUI = Gui.GetCurrent();
            if (GUI.IsChild(BackGround) == 1) { GUI.RemoveChild(BackGround); }
            if (GUI.IsChild(ImageGrid) == 1) 
            { 
                GUI.RemoveChild(ImageGrid);
                GUI.RemoveChild(TextGrid); 
                DeleteGrid();
            }
        }
        public void Show()
        {
            Gui GUI = Gui.GetCurrent();
            if (GUI.IsChild(BackGround) == 0) { GUI.AddChild(BackGround, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP); }
            if (GUI.IsChild(ImageGrid) == 0) 
            { 
                CreateGrid();
                GUI.AddChild(ImageGrid, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
                GUI.AddChild(TextGrid, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP); 
            }
        }

        public void ShutDown() { Hide(); BackGround.DeleteLater(); }
        private void DeleteGrid()
        {

            foreach (Widget Image in ImageList)
            {
                ImageGrid.RemoveChild(Image);
                Image.DeleteLater();
            }

            foreach (Widget Text in TextList)
            {
                TextGrid.RemoveChild(Text);
                Text.DeleteLater();
            }

            Map.Clear();
            ImageList.Clear(); TextList.Clear();
            ImageGrid.DeleteLater(); TextGrid.DeleteLater();
        }

        private void CreateBackground()
        {
            Gui GUI = Gui.GetCurrent();
            BackGround = new WidgetCanvas();
            int BNum = BackGround.AddPolygon();
            int x = 500, y = 300;
            ivec3[] Points =
            {
                new ivec3 (-x, -y, 0),
                new ivec3 ( x, -y, 0),
                new ivec3 ( x,  y, 0),
                new ivec3 (-x,  y, 0),
            };

            foreach (ivec3 point in Points) { BackGround.AddPolygonPoint(BNum, point); }
            BackGround.SetPolygonColor(BNum, new vec4(0, 0, 0, 0.8f));
            BackGround.SetPosition(
                (int)((GUI.Width * 0.5) - (x * 0.5)),
                (int)((GUI.Height * 0.5) - (y * 0.5)));
        }

        private void CreateGrid()
        {

            int ImageSize = 64, Column = 6, Padding = 5, InventorySize = Inventory.ArraySize();
            ImageGrid = new(Column, Padding, Padding); TextGrid = new(Column, Padding, Padding);
            Map = new(); ImageList = new(); TextList = new();
            ImageList.Capacity = InventorySize;
            TextList.Capacity = InventorySize;

            for (int i = 0; i < InventorySize; i++)
            {
                int _ID = Inventory.GetItem(i).x;
                Image _i = new(); _i.Load(Database.GetImagePath(_ID));
                WidgetSprite Image = new();

                Image.SetImage(_i);
                Image.Width = 64;
                Image.Height = 64;

                if (_ID != 0) { CreateCallbacks(Image); } // Only on actual Items
                Image.AddCallback(Gui.CALLBACK_INDEX.DRAG_DROP,OnDrops);

                Map[Image] = i;
                ImageList.Add(Image);
                ImageGrid.AddChild(Image);

                int _Amount = Inventory.GetItem(i).y;
                WidgetLabel Text = new();
                if(_ID != 0) { Text.Text = _Amount.ToString(); }
                Text.FontSize = 12;
                Text.FontColor = vec4.WHITE;
                Text.Width = 64;
                Text.Height = 64;

                TextList.Add(Text);
                TextGrid.AddChild(Text);
            }
            CreateThrowAway();
            int x = (ImageSize * Column) + (Column - 1) * Padding,
                y = MathLib.FloorToInt((float)InventorySize / Column); if(InventorySize % Column != 0) { y++; }
                y = (y * ImageSize) + (y - 1) * Padding;
            Gui GUI = Gui.GetCurrent();

            ImageGrid.SetPosition((int)((GUI.Width * 0.5) - (x * 0.5)), (int)((GUI.Height * 0.5) - (y * 0.5)));
            TextGrid.SetPosition((int)((GUI.Width * 0.5) - (x * 0.5)), (int)((GUI.Height * 0.5) - (y * 0.5)));
        }

        private void CreateThrowAway()
        {
            ThrowAway = new();
            Image _i = new(); _i.Load("Database/icons/ThrowAway.png");
            ThrowAway.SetImage(_i);
            ThrowAway.Width = 64;
            ThrowAway.Height = 64;
            ThrowAway.AddCallback(Gui.CALLBACK_INDEX.DRAG_DROP, OnDrops);

            Map[ThrowAway] = Inventory.ArraySize() + 1;
            ImageList.Add(ThrowAway);
            ImageGrid.AddChild(ThrowAway);
        }

        private void CreateCallbacks(WidgetSprite Image)
        {
            Image.AddCallback(Gui.CALLBACK_INDEX.ENTER,     () => OnHover(Image));
            Image.AddCallback(Gui.CALLBACK_INDEX.LEAVE,     () => OnLeave(Image));
            Image.AddCallback(Gui.CALLBACK_INDEX.CLICKED,   () => OnClick(Image));
        }

        void OnHover(WidgetSprite Image) { Image.Color = vec4.GREEN; }
        void OnLeave(WidgetSprite Image) { Image.Color = vec4.WHITE; }
        void OnClick(WidgetSprite Image) {

            int Pos = 0;
            if(Map.ContainsKey(Image)) { Pos = Map[Image]; }
            int ID = Inventory.GetItem(Pos).x,
                Amount = Inventory.GetItem(Pos).y,
                Value = Database.GetValue(ID);
            string Name = Database.GetName(ID);
            Log.Message($" Name: {Name}   ID: {ID}  Amount: {Amount}  Value: {Value} \n");
        }
        void OnDrops(Widget Image1, Widget Image2) {

            WidgetSprite _0 = Image1 as WidgetSprite, _1 = Image2 as WidgetSprite; 
            int Pos1 = 0, Pos2 = 0;
            if (Map.ContainsKey(_0)) { Pos1 = Map[_0]; }
            if (Map.ContainsKey(_1)) { Pos2 = Map[_1]; }

            if (Pos1 > Inventory.ArraySize())
            {
                ivec2 Item = Inventory.GetItem(Pos2);
                Inventory.Delete(Pos2);

                Node _node = World.LoadNode(Database.GetPrefabPath(Item.x));
                _node.GetProperty(0).ParameterPtr.GetChild(1).ValueInt = Item.y;
                _node.WorldPosition = Game.Player.Position + Game.Player.GetDirection();
            }

            else if (Pos2 > Inventory.ArraySize())
            {
                ivec2 Item = Inventory.GetItem(Pos1);
                Inventory.Delete(Pos1);

                Node _node = World.LoadNode(Database.GetPrefabPath(Item.x));
                _node.GetProperty(0).ParameterPtr.GetChild(1).ValueInt = Item.y;
                _node.WorldPosition = Game.Player.Position + Game.Player.GetDirection();
            }

            else { Inventory.Swap(Pos2, Pos1); }
            Hide(); Show();
        }
    }
}
