using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using UnigineApp.data.Scripts.AI;

public struct PathList { public List<Node> Paths; }

[Component(PropertyGuid = "049f940b8298d9ee0d44d8757a998f50a4251a6f")]
public class AIManager : Component
{
	public Node NavNode, MainCharacter;
	public AssetLinkNode AINode;
	public List<Node> Obstacles;
	public PathList[] PathLists;
	NavigationMaker MainNav;

	List<ShooterAI> AIList = new();
	bool StartAI = false;

	private void Init()
	{
		// write here code to be called on component initialization
		MainNav = new NavigationMaker
			(
				1,
				NavNode,
				Obstacles,
				PathLists[0].Paths
				);

		for (int i = 0; i < 1; i++)
		{
			List<dvec3> Paths = new();
			PathLists[i].Paths.ForEach(p => Paths.Add(p.WorldPosition));
			Paths = MainNav.GetModifiedPath(Paths);
			Node _AI = World.LoadNode(AINode.Path);
			_AI.WorldPosition = Paths[0];

			ShooterAI AI = GetComponent<ShooterAI>(_AI);
			AI.SetAI((vec3)Paths[i], Paths, MainCharacter);
			AIList.Add(AI);
		}
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame

		MainNav.RenderNavigation();
		MainNav.RenderObstacles();

		if (Input.IsKeyDown(Input.KEY.C) && !StartAI) { StartAI = true; }
		if (StartAI) { AIList.ForEach(p => p.UpdateMove()); }
	}
}