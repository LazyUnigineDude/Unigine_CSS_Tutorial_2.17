using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "9f8d63f96fc33d04a90ca493ee00e0d6a0f5a23d")]
public class ViewportClip : Component
{
	[ShowInEditor]
	private Player mainPlayer;

	[ShowInEditor]
	private Player objectPlayer;

	[ShowInEditor]
	private Node objectClip;

	private Viewport mainViewport;
	private Viewport objectViewport;
	private Texture texture;
	private RenderTarget renderTarget;
	private WidgetSprite ClipTexture;


	private void Init()
	{
		EngineWindow Engine = WindowManager.MainWindow;

		// Renders
		renderTarget = new RenderTarget();
		mainViewport = new Viewport();
		objectViewport = new Viewport();
		objectViewport.NodeLightUsage = Viewport.USAGE_WORLD_LIGHT;

		// Texture Maker
		texture = new Texture();												// must flag 
		texture.Create2D(Engine.Size.x, Engine.Size.y, Texture.FORMAT_RGBA8, Texture.FORMAT_USAGE_RENDER);
        
		// Texture Displayer
		var gui = Gui.GetCurrent();
        ClipTexture = new WidgetSprite(gui);
        gui.AddChild(ClipTexture, Gui.ALIGN_OVERLAP | Gui.ALIGN_BACKGROUND);
	}
	
	private void Update()
	{
		// Get Clip View and make to Texture

		// Align and Enable Viewport Renders
        objectPlayer.WorldTransform = mainPlayer.Camera.IModelview;
		renderTarget.BindColorTexture(0, texture); // Capture Render into Buffer
		renderTarget.Enable();
		mainViewport.Render(mainPlayer.Camera);		// Render Regular Viewport
		objectViewport.RenderNode(objectPlayer.Camera, objectClip);	// Render Object
		renderTarget.Disable();
		renderTarget.UnbindAll();// Remove Capture
		// Render Texture into Widget to display on mainPlayer
		ClipTexture.SetRender(texture);
    }

	private void Shutdown()
	{
		renderTarget.DeleteLater();
		texture.DeleteLater();
		mainViewport.DeleteLater();
		objectViewport.DeleteLater();
	}
}
