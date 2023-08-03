using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "dbcc504aa5068321043bc20c35f495fb131f931e")]
public class PhysicsController : Component
{
	[ShowInEditor]
	private float Speed, MaxSpeed, SideSpeed;
	
	public enum DIRECTIONS { FORWARD, BACKWARD, LEFT, RIGHT };
	float RunAddition;
	BodyRigid MainCharacter;
	Node RigidNode;

	public void Initialize(Node _RigidNode)
	{
		RigidNode = _RigidNode;
		MainCharacter = RigidNode.ObjectBodyRigid;
		MainCharacter.MaxLinearVelocity = MaxSpeed;
		RunAddition = 2f;
	}

	public void Move(DIRECTIONS Direction) 
	{
		switch (Direction)
		{
			case DIRECTIONS.FORWARD:  MainCharacter.AddLinearImpulse(RigidNode.GetWorldDirection(MathLib.AXIS.Y)  * Speed     * RunAddition);break;
			case DIRECTIONS.BACKWARD: MainCharacter.AddLinearImpulse(RigidNode.GetWorldDirection(MathLib.AXIS.NY) * Speed	  * RunAddition);break;
			case DIRECTIONS.LEFT:     MainCharacter.AddLinearImpulse(RigidNode.GetWorldDirection(MathLib.AXIS.NX) * SideSpeed * RunAddition);break;
			case DIRECTIONS.RIGHT:	  MainCharacter.AddLinearImpulse(RigidNode.GetWorldDirection(MathLib.AXIS.X)  * SideSpeed * RunAddition);break;
			default: break;
		}
	}

	public void Run(bool isRunning, float LerpSpeed) => MainCharacter.MaxLinearVelocity =
		(isRunning) ?
		MathLib.Clamp(MainCharacter.MaxLinearVelocity + LerpSpeed, MaxSpeed * 0.5f, MaxSpeed * 2) :
        MathLib.Clamp(MainCharacter.MaxLinearVelocity - LerpSpeed, MaxSpeed * 0.5f, MaxSpeed * 2);

	public void AutoRotate(Player Camera)
	{
		vec3 CameraView, PlayerView;

		CameraView = Camera.GetWorldDirection();
		PlayerView = node.GetWorldDirection(MathLib.AXIS.Y);

		float Angle = MathLib.Angle(CameraView, PlayerView, node.GetWorldDirection(MathLib.AXIS.Z));
		MainCharacter.AddAngularImpulse(node.GetWorldDirection() * Angle);
	}

	public double getSpeed()
	{
		double speed = MathLib.Pow2(MainCharacter.LinearVelocity.x) + MathLib.Pow2(MainCharacter.LinearVelocity.y) + MathLib.Pow2(MainCharacter.LinearVelocity.z);
		speed = MathLib.Sqrt(speed);
		return speed;
	}
}