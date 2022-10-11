using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "dbcc504aa5068321043bc20c35f495fb131f931e")]
public class PhysicsController : Component
{
	public float Speed, MaxSpeed, SideSpeed;
	private float RunAddition;

	BodyRigid MainCharacter;

	private void Init()
	{
		MainCharacter = node.ObjectBodyRigid;
		MainCharacter.MaxLinearVelocity = MaxSpeed;
		RunAddition = 1f;
	}

	private void UpdatePhysics()
	{
		if (Input.IsKeyPressed(Input.KEY.LEFT_SHIFT))
		{ RunAddition = Math.Clamp(RunAddition += Game.IFps * 5, 1, 3f); }
		else RunAddition = Math.Clamp(RunAddition -= 2 * Game.IFps * 5, 1, 3f);

		if (Input.IsKeyPressed(Input.KEY.W))
		{ MainCharacter.AddLinearImpulse(node.GetWorldDirection(MathLib.AXIS.Y) * Speed * RunAddition); }

		if (Input.IsKeyPressed(Input.KEY.S))
		{ MainCharacter.AddLinearImpulse(node.GetWorldDirection(MathLib.AXIS.NY) * Speed); }

		if (Input.IsKeyPressed(Input.KEY.D))
		{ MainCharacter.AddLinearImpulse(node.GetWorldDirection(MathLib.AXIS.X) * SideSpeed * RunAddition); }

		if (Input.IsKeyPressed(Input.KEY.A))
		{ MainCharacter.AddLinearImpulse(node.GetWorldDirection(MathLib.AXIS.NX) * SideSpeed * RunAddition); }
		AutoRotate();
	}

	void AutoRotate()
	{
		vec3 CameraView, PlayerView;

		CameraView = Game.Player.GetWorldDirection();
		PlayerView = node.GetWorldDirection(MathLib.AXIS.Y);

		float Angle = MathLib.Angle(CameraView, PlayerView, node.GetWorldDirection(MathLib.AXIS.Z));
		MainCharacter.AddAngularImpulse(node.GetWorldDirection() * Angle);
	}
}