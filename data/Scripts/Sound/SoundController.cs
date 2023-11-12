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

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "6152fc53abde8cfe33e0eda8c42d2f389f5b9ff1")]
public class SoundController : Component
{
	public AssetLink SoundFile;
	SoundSource Source;

	public void Initialize()
	{
		// write here code to be called on component initialization
		Source = node as SoundSource;
		Source.SampleName = SoundFile.AbsolutePath;
	}

	public void PlaySound() => Source.Play();
	public void StopSound() => Source.Stop();
	public bool IsPlaying() => Source.IsPlaying;

}