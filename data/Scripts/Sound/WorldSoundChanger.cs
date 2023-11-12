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

[Component(PropertyGuid = "92ed92a951c124a06dd9cc30b9056eaba4bfdd96")]
public class WorldSoundChanger : Component
{
	public AssetLink SoundFile;
	public Node ObjectDetect;

	SoundSource Source;
	WorldTrigger Trigger;

	private void Init()
	{
		// write here code to be called on component initialization
		Trigger = node as WorldTrigger;
		Trigger.AddEnterCallback(StartSong);
		Log.Message("Hi");
	}

	private void StartSong(Node Node)
	{
		Source = Node as SoundSource;

		string one = Source.SampleName, two = SoundFile.AbsolutePath;

        if (one != two)
		{
			Source.SampleName = SoundFile.AbsolutePath;
			Source.Play();
		}
	}
}