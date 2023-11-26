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

[Component(PropertyGuid = "5cb22be490c5032a195c6f3fd77b10b9b92ca6c0")]
public class MainMenu : Component
{
	Gui GUI;
	WidgetButton NewGame, LoadAutoSave, Load1, Load2, Quit;
	SaveManger Save;

	private void Init()
	{
		// write here code to be called on component initialization
		GUI = Gui.GetCurrent();
		Save = new SaveManger(node);

        NewGame = new WidgetButton("New Game");
        NewGame.Width = 150;
        NewGame.SetPosition(150, 10);
        NewGame.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { World.LoadWorld("NewGame.world"); });

        LoadAutoSave = new WidgetButton("AutoSave");
        LoadAutoSave.Width = 150;
        LoadAutoSave.SetPosition(150, 30);

        Load1 = new WidgetButton("Load 1");
        Load1.Width = 150;
        Load1.SetPosition(150, 50);

        Load2 = new WidgetButton("Load 2");
        Load2.Width = 150;
        Load2.SetPosition(150, 70);

        Quit = new WidgetButton("Quit");
        Quit.Width = 150;
        Quit.SetPosition(150, 90);
        Quit.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Engine.Quit(); });

        if (Save.FileExist(SaveManger.DATA.AUTOSAVE)) 
            {   LoadAutoSave.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Load(SaveManger.DATA.AUTOSAVE); }); }
        else    LoadAutoSave.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Log.Error("Does Not Exist \n"); });

        if (Save.FileExist(SaveManger.DATA.SAVE1))
            {   Load1.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Load(SaveManger.DATA.SAVE1); }); }
        else    Load1.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Log.Error("Does Not Exist \n"); });

        if (Save.FileExist(SaveManger.DATA.SAVE2))
            {   Load2.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Save.Load(SaveManger.DATA.SAVE2); }); }
        else    Load2.AddCallback(Gui.CALLBACK_INDEX.CLICKED, () => { Log.Error("Does Not Exist \n"); });

        GUI.AddChild(NewGame, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(LoadAutoSave, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(Load1, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(Load2, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(Quit, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
    }
	
	private void Shutdown()
	{
        GUI = Gui.GetCurrent();

        if (GUI.IsChild(NewGame) == 1) { GUI.RemoveChild(NewGame); NewGame.DeleteLater(); }
        if (GUI.IsChild(LoadAutoSave) == 1) { GUI.RemoveChild(LoadAutoSave); LoadAutoSave.DeleteLater(); }
        if (GUI.IsChild(Load1) == 1) { GUI.RemoveChild(Load1); Load1.DeleteLater(); }
        if (GUI.IsChild(Load2) == 1) { GUI.RemoveChild(Load2); Load2.DeleteLater(); }
        if (GUI.IsChild(Quit) == 1) { GUI.RemoveChild(Quit); Quit.DeleteLater(); }
    }
}