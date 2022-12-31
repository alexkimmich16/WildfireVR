using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HandDebugger : MonoBehaviour
{
    public bool ShouldDebug;
    public Side side;
    public SkinnedMeshRenderer hand;
    public Spell currentSpell;
    public List<Material> FalseTrue;
    //public FireController fire;
    //public FireballController Fireball;
    //public BlockController Fireball;
    //public FireballController Fireball;
    //public Debug
    void Start()
    {
        if (currentSpell == Spell.Fireball)
        {
            //Fireball.gameObject.GetComponent<LearningAgent>().NewState += SetNew;
            AIMagicControl.instance.Fireballs[(int)side].RealNewState += SetNew;
        }
        else if (currentSpell == Spell.Flames)
        {
            AIMagicControl.instance.Flames[(int)side].RealNewState += SetNew;
            //fire.NewRealState += SetNew;
            //fire.frames.RealNewState += SetNew;
        }
        else if (currentSpell == Spell.Block)
        {
            AIMagicControl.instance.Blocks[(int)side].RealNewState += SetNew;
        }
        else if (currentSpell == Spell.Redirect)
        {
            //AIMagicControl.instance.Absorbs[(int)side].RealNewState += SetNew;
        }
    }
    private void Update()
    {
        hand.gameObject.SetActive(ShouldDebug);
    }
    public void SetNew(bool State)
    {
        hand.material = FalseTrue[Convert.ToInt32(State)];
    }
}
