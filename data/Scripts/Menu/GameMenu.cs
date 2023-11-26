/* Copyright (C) 2005-2023, UNIGINE. All rights reserved.
*
* This file is a part of the UNIGINE 2 SDK.
*
* Your use and / or redistribution of this software in source and / or
* binary form, with or without modification, is subject to: (i) your
* ongoing acceptance of and compliance with the terms and conditions of
* the UNIGINE License Agreement; and (ii) your inclusion of this notice
* in any version of this software that you use or redistribute.
* A copy of the UNIGINE License Agreement is available by contacting
* UNIGINE. at http://unigine.com/
*/

using Unigine;
using UnigineApp.data.Scripts.Menu;

[Component(PropertyGuid = "039c1f9ef7c4da4ffd6ac8cf24cd08dd2244f0cb")]
public class GameMenu : Component
{
	[ShowInEditor] bool NewGame;
	WidgetButton MainMenu, SaveGame, SaveGame2, SaveGame3, Quit;
	Gui GUI;
	SaveManger Save;

	private void Init()
	{
		GUI = Gui.GetCurrent();
        Save = new SaveManger(node);

		MainMenu = new WidgetButton("Main Menu");
		MainMenu.Width = 150;
		MainMenu.SetPosition(150, 10);
		MainMenu.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { World.LoadWorld("CS_Check.world"); });

        SaveGame = new WidgetButton("AutoSave");
        SaveGame.Width = 150;
        SaveGame.SetPosition(150, 30);
        SaveGame.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Save(SaveManger.DATA.AUTOSAVE); });

        SaveGame2 = new WidgetButton("Save 1");
        SaveGame2.Width = 150;
        SaveGame2.SetPosition(150, 50);
        SaveGame2.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Save(SaveManger.DATA.SAVE1); });

        SaveGame3 = new WidgetButton("Save 2");
        SaveGame3.Width = 150;
        SaveGame3.SetPosition(150, 70);
        SaveGame3.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Save(SaveManger.DATA.SAVE2); });

        Quit = new WidgetButton("Quit");
        Quit.Width = 150;
        Quit.SetPosition(150, 90);
        Quit.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Engine.Quit(); });

        GUI.AddChild(MainMenu, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(SaveGame, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(SaveGame2, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(SaveGame3, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(Quit, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        if (!NewGame) { Save.LoadObjectsIntoWorld(); }
    }
	
	private void Shutdown()
	{
        GUI = Gui.GetCurrent();

        if (GUI.IsChild(MainMenu) == 1)  { GUI.RemoveChild(MainMenu); MainMenu.DeleteLater(); }
        if (GUI.IsChild(SaveGame) == 1)  { GUI.RemoveChild(SaveGame); SaveGame.DeleteLater(); }
        if (GUI.IsChild(SaveGame2) == 1) { GUI.RemoveChild(SaveGame2); SaveGame2.DeleteLater(); }
        if (GUI.IsChild(SaveGame3) == 1) { GUI.RemoveChild(SaveGame3); SaveGame3.DeleteLater(); }
        if (GUI.IsChild(Quit) == 1)      { GUI.RemoveChild(Quit); Quit.DeleteLater(); }

       // Save.Save(SaveManger.DATA.AUTOSAVE); // AUTOSAVE Feature
    }
}