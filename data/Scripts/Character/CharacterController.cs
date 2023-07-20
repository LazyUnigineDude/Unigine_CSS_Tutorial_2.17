using Unigine;

[Component(PropertyGuid = "ef5f1d8ee1c70fe29751ee4c684438c6d56cffb7")]
public class CharacterController : Component {

	[ShowInEditor]
	private Node 
		AnimationNode,
		PhysicsNode,
		HealthBarNode,
		InventoryNode,
		InteractorNode,
		GunNode;

	AnimationController Animation;
	PhysicsController Physics;
	HealthBar Healthbar;
	InventoryController Inventory;
	Interactor Interact;
	GunHandler Gun;

	HUDMaker HUD;
	bool isUIOpen;

	private void Init()
	{
		// write here code to be called on component initialization

		Animation = GetComponent<AnimationController>(AnimationNode);
		Physics   =	GetComponent<PhysicsController>(PhysicsNode);
		Healthbar = GetComponent<HealthBar>(HealthBarNode);
        Inventory = GetComponent<InventoryController>(InventoryNode);
		Interact  = GetComponent<Interactor>(InteractorNode);
		Gun		  = GetComponent<GunHandler>(GunNode);

		Animation.Initialize(node);
		Physics.Initialize(node);
		Inventory.Initialize();
		Interact.Initialize(Game.Player);
		Gun.Initialize(Game.Player);

		HUD = GetComponent<HUDMaker>(World.GetNodeByID(HUDMaker.NODE_ID));
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame

		if(!isUIOpen) { 
			if (Input.IsKeyPressed(Input.KEY.W) && Input.IsKeyPressed(Input.KEY.LEFT_SHIFT)) { Animation.ChangeAnim(AnimationController.ANIM_STATE.RUN); }
			if (Input.IsKeyPressed(Input.KEY.W)) { Animation.ChangeAnim(AnimationController.ANIM_STATE.WALK); }
			else if (Input.IsKeyPressed(Input.KEY.S)) { Animation.ChangeAnim(AnimationController.ANIM_STATE.REVERSE_WALK); }
			else if (Input.IsKeyPressed(Input.KEY.A)) { Animation.ChangeAnim(AnimationController.ANIM_STATE.SIDE_WALK_L); }
			else if (Input.IsKeyPressed(Input.KEY.D)) { Animation.ChangeAnim(AnimationController.ANIM_STATE.SIDE_WALK_R); }
			else Animation.ChangeAnim(AnimationController.ANIM_STATE.IDLE);
        }

		if(Input.IsKeyDown(Input.KEY.Q)) { isUIOpen = (isUIOpen) ? Inventory.Hide() : Inventory.Show(); }
		 
		if(Interact.DetectItem())
		{
			if (Input.IsKeyDown(Input.KEY.E)) 
			{
				ivec2 Item = Interact.GetItem();
				DatabaseController Db = GetComponent<DatabaseController>(World.GetNodeByID(DatabaseController.NODE_ID));
                DatabaseController.ITEM_TYPE Type = Db.GetType(Item.x);

				if (Type == DatabaseController.ITEM_TYPE.GUN) Gun.GetGun(Item);
				else Inventory.AddIntoInventory(Item); 
			}
		}

		if(Input.IsKeyDown(Input.KEY.Y)) 
		{
			if (!Gun.isHolding) ChangeState(AnimationController.SHOOTER_STATE.EQUIP);
			else ChangeState(AnimationController.SHOOTER_STATE.NORMAL);
		}

		if (Gun.isHolding)
		{
			if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))  { Gun.Shoot(Game.Time); HUD.UpdateGun(Gun.GUIGunValue()); }
			if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.RIGHT)) { ChangeState(AnimationController.SHOOTER_STATE.AIMED); }
			if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.RIGHT))   { ChangeState(AnimationController.SHOOTER_STATE.EQUIP); }
			if (Input.IsKeyPressed(Input.KEY.R)) { Gun.Reload(); HUD.UpdateGun(Gun.GUIGunValue()); }
		}
		Animation.UpdateAnimations(Game.IFps, Game.Time);
	}

    private void UpdatePhysics()
	{
		if (!isUIOpen)
		{
			if (Input.IsKeyPressed(Input.KEY.LEFT_SHIFT)) { Physics.Run(true, Game.IFps * 2); } else Physics.Run(false, Game.IFps * 5);
			if (Input.IsKeyPressed(Input.KEY.W)) { Physics.Move(PhysicsController.DIRECTIONS.FORWARD); }
			else if (Input.IsKeyPressed(Input.KEY.S)) { Physics.Move(PhysicsController.DIRECTIONS.BACKWARD); }
			else if (Input.IsKeyPressed(Input.KEY.A)) { Physics.Move(PhysicsController.DIRECTIONS.LEFT); }
			else if (Input.IsKeyPressed(Input.KEY.D)) { Physics.Move(PhysicsController.DIRECTIONS.RIGHT); }
            Physics.AutoRotate(Game.Player);
        }
    }

	private void Shutdown() { Inventory.ShutDown(); Interact.ShutDown(); }

	private void ChangeState(AnimationController.SHOOTER_STATE sHOOTER_STATE)
	{
		switch (sHOOTER_STATE)
		{
			case AnimationController.SHOOTER_STATE.NORMAL:
				Animation.ChangeState(AnimationController.SHOOTER_STATE.NORMAL);
				if(Gun.isHolding)
				{
					Gun.UnEquip();
					HUD.HideGun();
				}
				break;
			case AnimationController.SHOOTER_STATE.EQUIP:
				if(!Gun.isHolding) { Gun.Equip(); }
				if(Gun.isGrabbed) { Animation.ChangeState(sHOOTER_STATE); HUD.UpdateGun(Gun.GUIGunValue()); }
				break;
			case AnimationController.SHOOTER_STATE.AIMED:
				if (Gun.isHolding) { Animation.ChangeState(sHOOTER_STATE); }
				break;
			default:
				break;
		}
	}

}