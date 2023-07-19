using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "84a5f9797713d484f00156135ca6e456bc48cfbd")]
public class HUDMaker : Component
{
    [ShowInEditor]
    private Node HealthbarNode;

    public AssetLink _image, HealthImage;
    Gui GUI;
    WidgetCanvas Canvas;
    WidgetSprite Sprite;
    WidgetLabel CurAmount, MaxAmount;
    WidgetGridBox HealthGrid;
    int Width, Height, CurrentHealth;
    HealthBar MainCharHealth;
    public const int NODE_ID = 8058553;

    private void Init()
    {
        // write here code to be called on component initialization
        GUI = Gui.GetCurrent();

        MainCharHealth = GetComponent<HealthBar>(HealthbarNode);
        CurrentHealth = MainCharHealth.ShowHealth();
        Width = GUI.Width;
        Height = GUI.Height;

        Canvas = new();

        //int x = Canvas.AddText(1);
        //Canvas.SetTextText(x, "SAMPLE_TEXT");
        //Canvas.SetTextColor(x, new vec4(1, 1, 1, 0.5));
        //Canvas.SetTextSize(x, 30);
        //Canvas.SetTextPosition(x, new vec2((Width / 2) - (Canvas.GetTextWidth(x) / 2), Height / 2 - (Canvas.GetTextHeight(x) / 2)));

        int y = Canvas.AddPolygon(0);
        Canvas.SetPolygonColor(y, new vec4(0, 0, 0, 0.5));
        Canvas.AddPolygonPoint(y, new vec3(0, 700, 0));
        Canvas.AddPolygonPoint(y, new vec3(450, 700, 0));
        Canvas.AddPolygonPoint(y, new vec3(450, 800, 0));
        Canvas.AddPolygonPoint(y, new vec3(0, 800, 0));

        Sprite = new();

        int z = Sprite.AddLayer();
        Image _i = new(); _i.Load(_image.AbsolutePath);
        Sprite.SetImage(_i);
        Sprite.Width = 20;
        Sprite.Height = 20;
        Sprite.SetPosition((Width / 2) - 10, (Height / 2) - 10);

        //GRID for health

        HealthGrid = new WidgetGridBox(20, 1, 1);
        GainHealth(CurrentHealth);
        HealthGrid.SetPosition(0, 700);

        CurAmount = new(); MaxAmount = new();

        GUI.AddChild(Canvas, Gui.ALIGN_EXPAND);
        GUI.AddChild(Sprite, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);

        GUI.AddChild(CurAmount, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(MaxAmount, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
        GUI.AddChild(HealthGrid, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
    }

    private void Update()
    {
        // write here code to be called before updating each render frame

        GUI = Gui.GetCurrent();

        if (CurrentHealth > MainCharHealth.ShowHealth())
        {
            LoseHealth(CurrentHealth - MainCharHealth.ShowHealth());
            CurrentHealth = MainCharHealth.ShowHealth();
        }
    }

    public void UpdateGun(ivec2 GunVal)
    {

        CurAmount.Text = GunVal.x.ToString();
        CurAmount.FontSize = 48;
        CurAmount.SetPosition(Width - 160, 20);


        MaxAmount.Text = GunVal.y.ToString();
        MaxAmount.FontSize = 36;
        MaxAmount.SetPosition(Width - 80, 20);
    }

    public void HideGun()
    {
        CurAmount.Text = " ";
        MaxAmount.Text = " ";
    }

    void LoseHealth(int Amount)
    {

        for (int i = 0; i < Amount; i++)
        {
            HealthGrid.RemoveChild(HealthGrid.GetChild(0));
        }
    }

    void GainHealth(int Amount)
    {

        for (int i = 0; i < Amount; i++)
        {
            WidgetSprite _image = new();
            Image _i2 = new(); _i2.Load(HealthImage.AbsolutePath);
            _image.SetImage(_i2);
            _image.Width = 20;
            _image.Height = 20;
            HealthGrid.AddChild(_image);
        }
    }

    void Shutdown()
    {
        if (GUI.IsChild(Canvas) == 1) { GUI.RemoveChild(Canvas); Canvas.DeleteLater(); }
        if (GUI.IsChild(CurAmount) == 1) { GUI.RemoveChild(CurAmount); CurAmount.DeleteLater(); }
        if (GUI.IsChild(MaxAmount) == 1) { GUI.RemoveChild(MaxAmount); MaxAmount.DeleteLater(); }
        if (GUI.IsChild(HealthGrid) == 1) { GUI.RemoveChild(HealthGrid); HealthGrid.DeleteLater(); }
    }
}