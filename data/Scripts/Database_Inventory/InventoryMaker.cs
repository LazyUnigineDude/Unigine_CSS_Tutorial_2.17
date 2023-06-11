using Unigine;

namespace UnigineApp.data.Scripts.Database_Inventory
{
    internal class InventoryMaker
    {
        private PropertyParameter _propertyParameter;

        public InventoryMaker() { }
        public InventoryMaker(PropertyParameter Parameter) => _propertyParameter = Parameter.GetChild(0);
        
        public int ArraySize() { return _propertyParameter.ArraySize; }

        public bool Add(ivec2 Value) { return Add(0, Value); }

        public bool Add(int Pos, ivec2 Value) {

            if (Pos > ArraySize()) { return false; }
            ivec2 Item = GetItem(Pos);

            if (Item.x == 0) { return SetItem(Pos, Value); }
            if (Item.x == Value.x)
            {
                ivec2 _Value = new ivec2( Item.x, Item.y + Value.y);
                return SetItem(Pos, _Value);
            }
            else { return Add(Pos + 1, Value); }
        }

        public ivec2 Swap(int Pos, ivec2 Value) {

            ivec2 _Item = Delete(Pos);
            SetItem(Pos, Value);
            return _Item;
        }

        public void Swap(int Pos1, int Pos2) {

            ivec2 _Item1 = Delete(Pos1);
            if(GetItem(Pos2).x == _Item1.x) { Add(Pos2, _Item1); }
            else
            {
                ivec2 _Item2 = Delete(Pos2);
                SetItem(Pos1, _Item2);
                SetItem(Pos2, _Item1);
            }
        }

        public ivec2 Delete(int Pos) { 
            
            ivec2 _Item = GetItem(Pos);
            SetItem(Pos, ivec2.ZERO); 
            return _Item;
        }

        public ivec2 GetItem(int Pos) { return _propertyParameter.GetChild(Pos).ValueIVec2; }
        private bool SetItem(int Pos, ivec2 Value) { _propertyParameter.ValueIVec2.Set(Value); return true; }
    }
}
