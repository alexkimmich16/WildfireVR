using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HandDebugger : MonoBehaviour
{
    public SkinnedMeshRenderer hand;
    public Spell currentSpell;
    public List<Material> FalseTrue;
    //public Debug
    void Start()
    {
        if (currentSpell == Spell.Fireball)
        {
            FireballController.instance.gameObject.GetComponent<LearningAgent>().NewState += SetNew;
        }
        else if (currentSpell == Spell.Flames)
        {
            FireController.instance.NewRealState += SetNew;
        }
    }

    public void SetNew(bool State)
    {
        hand.material = FalseTrue[Convert.ToInt32(State)];
    }
}
