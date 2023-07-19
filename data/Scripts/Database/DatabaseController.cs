using Unigine;

[Component(PropertyGuid = "081d46c939c0a194b30c1fe345ec03e6346eca0d")]
public class DatabaseController : Component
{
    [ShowInEditor]
    private Node Property;
    PropertyParameter Parameter;
    public const int NODE_ID = 842575985;
    public enum ITEM_TYPE { DEFAULT, GUN }

    public class GUNPROP { public string BulletPath; public int Damage, RoF, Reload; }
    private void Init() => Parameter = Property.GetProperty(0).ParameterPtr.GetChild(0);

    public string GetName(int ID) { return Parameter.GetChild(ID).GetChild(0).ValueString; }
    public int GetValue(int ID) { return Parameter.GetChild(ID).GetChild(1).ValueInt; }
    public string GetImagePath(int ID) { return Parameter.GetChild(ID).GetChild(2).ValueFile; }
    public string GetPrefabPath(int ID) { return Parameter.GetChild(ID).GetChild(3).ValueFile; }
    public ITEM_TYPE GetType(int ID) { return (ITEM_TYPE)Parameter.GetChild(ID).GetChild(4).ValueSwitch; }
    public GUNPROP GetGunProperty(int ID)
    {
        PropertyParameter P = Parameter.GetChild(ID).GetChild(5);
        GUNPROP Gun = new();
        Gun.BulletPath =    P.GetChild(0).ValueFile;
        Gun.Damage =        P.GetChild(1).ValueInt;
        Gun.RoF =           P.GetChild(2).ValueInt;
        Gun.Reload =        P.GetChild(3).ValueInt;
        return Gun;
    }
}