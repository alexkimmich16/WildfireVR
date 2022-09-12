using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HandDebugger : MonoBehaviour
{
    public SkinnedMeshRenderer hand;
    public Spell currentSpell;
    public List<Material> FalseTrue;
    public FireController fire;
    public FireballController Fireball;
    //public Debug
    void Start()
    {
        if (currentSpell == Spell.Fireball)
        {
            Fireball.gameObject.GetComponent<LearningAgent>().NewState += SetNew;
        }
        else if (currentSpell == Spell.Flames)
        {
            fire.NewRealState += SetNew;
        }
    }

    public void SetNew(bool State)
    {
        hand.material = FalseTrue[Convert.ToInt32(State)];
    }
}
