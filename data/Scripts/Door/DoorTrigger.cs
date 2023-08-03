using Unigine;

[Component(PropertyGuid = "2d075594c853254ad9c70233972d4bb6b5e6cfee")]
public class DoorTrigger : Component {

	public Node DoorNode;
	public dvec3 ClosePos, OpenPos;
	public Node PhysicalTriggerNode;

	protected void Init() => InitTrigger(); 

	protected void OnEnter(Body body) => DoorNode.WorldPosition = OpenPos;
	protected void OnLeave(Body body) => DoorNode.WorldPosition = ClosePos;
	protected void InitTrigger() 
	{
		PhysicalTrigger trigger = PhysicalTriggerNode as PhysicalTrigger;
		if (trigger != null)
		{
			trigger.AddEnterCallback(OnEnter);
			trigger.AddLeaveCallback(OnLeave);
        }
		Log.Message("AddedTrigger\n");
	}
}