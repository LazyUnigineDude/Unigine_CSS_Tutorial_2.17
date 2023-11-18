using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unigine;

[Component(PropertyGuid = "364df5f913b6a70452a50693d3c9dad6c9c6543d")]
public class Menu : Component
{
	public bool isMainMenu;
	public string WorldName;
	public AssetLink WorldAsset,WaterAsset;

	WidgetButton Button, Quit, Water;
	Gui GUI = Gui.GetCurrent();
	bool isPaused = false;

	private void Init()
	{
		// write here code to be called on component initialization
		Button = new();
		Button.Text = WorldName;
		Button.FontSize = 75;
		Button.ButtonColor = vec4.WHITE;
		Button.AddCallback(Gui.CALLBACK_INDEX.CLICKED, OnClicked);
		Button.SetPosition(200, 100);

        Water = new();
        Water.Text = "Water";
        Water.FontSize = 75;
        Water.ButtonColor = vec4.WHITE;
        Water.AddCallback(Gui.CALLBACK_INDEX.CLICKED, OnClickedW);
        Water.SetPosition(200, 600);

        Quit = new();
		Quit.Text = "Quit";
		Quit.FontSize = 75;
		Quit.ButtonColor = vec4.WHITE;
		Quit.AddCallback(Gui.CALLBACK_INDEX.CLICKED, OnQuit);
		Quit.SetPosition(200, 300);

		MenuOpened(isMainMenu);
	}

	private void Update()
	{
		// write here code to be called before updating each render frame

		if (!isMainMenu)
		{
			if (Input.IsKeyDown(Input.KEY.ESC))
			{
				isPaused = (isPaused) ? false : true;
				MenuOpened(isPaused);
			}
		}
	}

	private void MenuOpened(bool isPaused)
	{
		if (isPaused)
		{
			if (GUI.IsChild(Button) == 0) GUI.AddChild(Button);
			if (GUI.IsChild(Quit) == 0) GUI.AddChild(Quit);
			if (GUI.IsChild(Water) == 0) GUI.AddChild(Water);
		}
		else
		{
			if (GUI.IsChild(Button) == 1) GUI.RemoveChild(Button);
			if (GUI.IsChild(Quit) == 1) GUI.RemoveChild(Quit);
			if (GUI.IsChild(Water) == 1) GUI.RemoveChild(Water);
		}
	}

	private void OnClickedW() { MenuOpened(false); Water.DeleteLater(); Button.DeleteLater(); Quit.DeleteLater(); World.LoadWorld(WaterAsset.Path); }
	private void OnClicked() { MenuOpened(false);  Water.DeleteLater(); Button.DeleteLater(); Quit.DeleteLater(); World.LoadWorld(WorldAsset.Path); }
	private void OnQuit() => Engine.Quit();
}