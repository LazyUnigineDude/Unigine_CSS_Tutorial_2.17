using Unigine;

[Component(PropertyGuid = "7a71d838d1846c7883c6db07e1877f0cda0ad232")]
public class DoorOpener : DoorTrigger
{
	public Node ButtonNode;
	bool isOpen = false;

	protected void Init()
	{
		PhysicalTrigger trigger = PhysicalTriggerNode as PhysicalTrigger;
		if (trigger != null) 
		{
			trigger.AddEnterCallback(OnEnter);
		}
	}

	protected void OnEnter(Body body) => OpenDoor();
	
	public void OpenDoor()
	{
		isOpen = (!isOpen) ? true : false;
		if (isOpen) { DoorNode.WorldPosition = OpenPos; }
		else { DoorNode.WorldPosition = ClosePos; }
	}
}