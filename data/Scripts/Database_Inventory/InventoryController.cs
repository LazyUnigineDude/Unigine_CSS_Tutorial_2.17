using Unigine;
using UnigineApp.data.Scripts.Database_Inventory;

[Component(PropertyGuid = "185a98fa82665411d6c8fe3c0dcbc46b4e7d8ece")]
public class InventoryController : Component
{
	[ShowInEditor]
    private Node Property;
	private InventoryMaker Inventory;
	private InventoryGUI GUI;

    private void Init()
	{
		Inventory = new InventoryMaker(Property.GetProperty(0).ParameterPtr);
		DatabaseController _Data = GetComponent<DatabaseController>(World.GetNodeByID(DatabaseController.NODE_ID));
		GUI = new InventoryGUI(Inventory, _Data);
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		if(Input.IsKeyDown(Input.KEY.Y)) { GUI.Show(); }
		if(Input.IsKeyDown(Input.KEY.U)) { GUI.Hide(); }
	}

	private void Shutdown()
	{
		GUI.ShutDown();
	}
}