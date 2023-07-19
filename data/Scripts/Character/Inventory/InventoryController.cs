using System.Reflection.Metadata.Ecma335;
using Unigine;
using UnigineApp.data.Scripts.Database_Inventory;

[Component(PropertyGuid = "185a98fa82665411d6c8fe3c0dcbc46b4e7d8ece")]
public class InventoryController : Component
{
	[ShowInEditor]
	private Node Property;
	private InventoryMaker Inventory;
	private InventoryGUI GUI;

	public void Initialize()
	{
		Inventory = new InventoryMaker(Property.GetProperty(0).ParameterPtr);
		DatabaseController _Data = GetComponent<DatabaseController>(World.GetNodeByID(DatabaseController.NODE_ID));
		GUI = new InventoryGUI(Inventory, _Data);
	}

	public bool Show() { GUI.Show(); return true; }
	public bool Hide() { GUI.Hide(); return false; }
	public void AddIntoInventory(ivec2 Item) => Inventory.Add(Item);

	public void ShutDown() => GUI.ShutDown(); 
}