using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "eead75effc922a82737ce7f98584f104859ac893")]
public class AnimationController : Component
{
    public Property AnimationScripts;
    public Player MainCamera;

    float Weight;
    ObjectMeshSkinned MainCharacter;

    enum ANIM_STATE { IDLE = 0, WALK, REVERSE_WALK, SIDE_WALK_L, SIDE_WALK_R, RUN, COUNT }
    enum SHOOTER_STATE { NORMAL = 0, EQUIP, AIMED, COUNT }

    ANIM_STATE STATE = ANIM_STATE.IDLE, PREV_STATE = ANIM_STATE.IDLE;
    SHOOTER_STATE SHOOTERSTATE = SHOOTER_STATE.NORMAL;

    private void Init()
    {
        // write here code to be called on component initialization
        MainCharacter = node as ObjectMeshSkinned;
        MainCharacter.NumLayers = (int)ANIM_STATE.COUNT * (int)SHOOTER_STATE.COUNT;
        PropertyParameter Property = AnimationScripts.GetParameterPtr(0).GetChild(0);
        for (int i = 0; i < (int)ANIM_STATE.COUNT; i++)
        {
            int Temp = MainCharacter.AddAnimation(Property.GetChild(0).GetChild(i + 1).ValueFile);
            MainCharacter.SetAnimation(i, Temp);

            Temp = MainCharacter.AddAnimation(Property.GetChild(1).GetChild(i + 1).ValueFile);
            MainCharacter.SetAnimation(i + (int)ANIM_STATE.COUNT, Temp);

            Temp = MainCharacter.AddAnimation(Property.GetChild(2).GetChild(i + 1).ValueFile);
            MainCharacter.SetAnimation(i + ((int)ANIM_STATE.COUNT * 2), Temp);
        }
    }

    private void Update()
    {
        // write here code to be called before updating each render frame
        for (int i = 0; i < (int)ANIM_STATE.COUNT; i++)
        {
            MainCharacter.SetFrame(((int)SHOOTERSTATE * (int)ANIM_STATE.COUNT) + i, Game.Time * 30);
        }

        Weight = MathLib.Clamp(Weight + Game.IFps, 0f, 1f);
        ShooterChanger();
    }

    public void ChangeStateToEquipped() { SHOOTERSTATE = SHOOTER_STATE.EQUIP; }

    private void ShooterChanger()
    {
        switch (SHOOTERSTATE)
        {
            case SHOOTER_STATE.NORMAL:


                AnimChanger((int)SHOOTER_STATE.NORMAL * (int)ANIM_STATE.COUNT);
                break;


            case SHOOTER_STATE.EQUIP:

                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.RIGHT))
                {
                    SHOOTERSTATE = SHOOTER_STATE.AIMED;

                    ThirdPersonCamera x = MainCamera.GetComponent<ThirdPersonCamera>();
                    x.isAiming = true;
                }

                AnimChanger((int)SHOOTER_STATE.EQUIP * (int)ANIM_STATE.COUNT);
                break;


            case SHOOTER_STATE.AIMED:

                if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.RIGHT))
                {
                    SHOOTERSTATE = SHOOTER_STATE.EQUIP;

                    ThirdPersonCamera x = MainCamera.GetComponent<ThirdPersonCamera>();
                    x.isAiming = false;
                }

                AnimChanger((int)SHOOTER_STATE.AIMED * (int)ANIM_STATE.COUNT);
                break;
            default:
                break;
        }

    }

    void AnimChanger(int SHOOTERSTATE)
    {


        switch (STATE)
        {
            case ANIM_STATE.IDLE:
                if (Input.IsKeyPressed(Input.KEY.W) && Input.IsKeyPressed(Input.KEY.LEFT_SHIFT)) { ResetWeight(); STATE = ANIM_STATE.RUN; PREV_STATE = ANIM_STATE.IDLE; }
                if (Input.IsKeyPressed(Input.KEY.W)) { ResetWeight(); STATE = ANIM_STATE.WALK; PREV_STATE = ANIM_STATE.IDLE; }
                if (Input.IsKeyPressed(Input.KEY.S)) { ResetWeight(); STATE = ANIM_STATE.REVERSE_WALK; PREV_STATE = ANIM_STATE.IDLE; }
                if (Input.IsKeyPressed(Input.KEY.A)) { ResetWeight(); STATE = ANIM_STATE.SIDE_WALK_L; PREV_STATE = ANIM_STATE.IDLE; }
                if (Input.IsKeyPressed(Input.KEY.D)) { ResetWeight(); STATE = ANIM_STATE.SIDE_WALK_R; PREV_STATE = ANIM_STATE.IDLE; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.IDLE + SHOOTERSTATE, Weight * 2);
                break;
            case ANIM_STATE.WALK:
                if (Input.IsKeyUp(Input.KEY.W)) { ResetWeight(); STATE = ANIM_STATE.IDLE; PREV_STATE = ANIM_STATE.WALK; }
                if (Input.IsKeyPressed(Input.KEY.LEFT_SHIFT)) { ResetWeight(); STATE = ANIM_STATE.RUN; PREV_STATE = ANIM_STATE.WALK; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.WALK + SHOOTERSTATE, Weight * 4);
                break;
            case ANIM_STATE.REVERSE_WALK:
                if (Input.IsKeyUp(Input.KEY.S)) { ResetWeight(); STATE = ANIM_STATE.IDLE; PREV_STATE = ANIM_STATE.REVERSE_WALK; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.REVERSE_WALK + SHOOTERSTATE, Weight * 3);
                break;
            case ANIM_STATE.RUN:
                if (Input.IsKeyUp(Input.KEY.LEFT_SHIFT)) { ResetWeight(); STATE = ANIM_STATE.WALK; PREV_STATE = ANIM_STATE.RUN; }
                if (Input.IsKeyUp(Input.KEY.W)) { ResetWeight(); STATE = ANIM_STATE.IDLE; PREV_STATE = ANIM_STATE.RUN; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.RUN + SHOOTERSTATE, Weight * 5);
                break;
            case ANIM_STATE.SIDE_WALK_L:
                if (Input.IsKeyUp(Input.KEY.A)) { ResetWeight(); STATE = ANIM_STATE.IDLE; PREV_STATE = ANIM_STATE.SIDE_WALK_L; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.SIDE_WALK_L + SHOOTERSTATE, Weight * 3);
                break;
            case ANIM_STATE.SIDE_WALK_R:
                if (Input.IsKeyUp(Input.KEY.D)) { ResetWeight(); STATE = ANIM_STATE.IDLE; PREV_STATE = ANIM_STATE.SIDE_WALK_R; }
                MainCharacter.LerpLayer((int)ANIM_STATE.IDLE, (int)PREV_STATE + SHOOTERSTATE, (int)ANIM_STATE.SIDE_WALK_R + SHOOTERSTATE, Weight * 3);
                break;
            default:
                break;
        }
    }


    void ResetWeight() { Weight = 0; }
}