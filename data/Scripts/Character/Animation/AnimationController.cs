using Unigine;

[Component(PropertyGuid = "eead75effc922a82737ce7f98584f104859ac893")]
public class AnimationController : Component
{
    [ShowInEditor]
    private Property AnimationScripts;

    float Weight;
    ObjectMeshSkinned MainCharacter;

    public enum ANIM_STATE { IDLE = 1, WALK, REVERSE_WALK, SIDE_WALK_L, SIDE_WALK_R, RUN, COUNT }
    public enum SHOOTER_STATE { NORMAL = 0, EQUIP, AIMED, COUNT }

    ANIM_STATE A_STATE = ANIM_STATE.IDLE, PREV_A_STATE = ANIM_STATE.IDLE;
    SHOOTER_STATE S_STATE = SHOOTER_STATE.NORMAL, PREV_S_STATE;

    public void Initialize(Node SkinnedNode)
    {
        MainCharacter = SkinnedNode as ObjectMeshSkinned;
        MainCharacter.NumLayers = 2;
        UpdateAnims();
    }

    public bool isIdle() => A_STATE == ANIM_STATE.IDLE;
    public void UpdateAnimations(float iFPS, float Time)
    {
        for (int i = 0; i < 2; i++) { MainCharacter.SetFrame(i, Time * 30); }
        Weight = MathLib.Clamp(Weight + iFPS, 0f, 1f);
        LerpLayer();
    }

    public void ChangeState(SHOOTER_STATE STATE)
    {
        if (S_STATE != STATE) 
        {
            PREV_S_STATE = S_STATE;
            S_STATE = STATE;
            Weight = 0;
            UpdateAnims();
            PREV_S_STATE = STATE;
        }
    }

    public void ChangeAnim(ANIM_STATE STATE)
    {
        if (A_STATE != STATE) 
        {
            PREV_A_STATE = A_STATE;
            A_STATE = STATE;
            Weight = 0;
            UpdateAnims();
        }
    }

    void LerpLayer() => MainCharacter.LerpLayer(0, 0, 1, Weight);
    void UpdateAnims()
    {
        SetAnimation(PREV_S_STATE, PREV_A_STATE, 0);
        SetAnimation(S_STATE, A_STATE, 1);
    }

    string GetAnimation(SHOOTER_STATE _S_STATE, ANIM_STATE _A_STATE)
    {
        string File = "";
        PropertyParameter Property = AnimationScripts.GetParameterPtr(0).GetChild(0);
        switch (_S_STATE)
        {
            case SHOOTER_STATE.NORMAL: File = Property.GetChild(0).GetChild((int)_A_STATE).ValueFile; break;
            case SHOOTER_STATE.EQUIP:  File = Property.GetChild(1).GetChild((int)_A_STATE).ValueFile; break;
            case SHOOTER_STATE.AIMED:  File = Property.GetChild(2).GetChild((int)_A_STATE).ValueFile; break;
            default: break;
        }

        return File;
    }

    void SetAnimation(SHOOTER_STATE _S_STATE, ANIM_STATE _A_STATE, int layer)
    {
        string Path = GetAnimation(_S_STATE, _A_STATE);
        MainCharacter.SetAnimation(layer, MainCharacter.AddAnimation(Path));
    }
}