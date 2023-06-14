using Unigine;

[Component(PropertyGuid = "081d46c939c0a194b30c1fe345ec03e6346eca0d")]
public class DatabaseController : Component
{
    [ShowInEditor]
    private Node Property;
    PropertyParameter Parameter;
    public const int NODE_ID = 842575985;

    private void Init() => Parameter = Property.GetProperty(0).ParameterPtr.GetChild(0);

    public string GetName(int ID) { return Parameter.GetChild(ID).GetChild(0).ValueString; }
    public int GetValue(int ID) { return Parameter.GetChild(ID).GetChild(1).ValueInt; }
    public string GetImagePath(int ID) { return Parameter.GetChild(ID).GetChild(2).ValueFile; }
    public string GetPrefabPath(int ID) { return Parameter.GetChild(ID).GetChild(3).ValueFile; }
}