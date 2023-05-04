using Sirenix.OdinInspector;
using RestrictionSystem;
using System.Collections.Generic;
public abstract class SpellControlClass : SerializedMonoBehaviour
{
    public CurrentLearn Motion;
    public abstract void InitializeSpells();
    public abstract void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level);

    public void Start()
    {
        NetworkManager.OnInitialized += Initalize;
        
    }

    public void Initalize()
    {
        InitializeSpells();
        ConditionManager.instance.conditions.MotionConditions[(int)Motion - 1].OnNewState += RecieveNewState;
    }
}
