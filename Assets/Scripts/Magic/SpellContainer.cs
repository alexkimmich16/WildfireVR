using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Container/SpellContainer")]
public class SpellContainer : ScriptableObject
{
    public List<SpellHolder> Spells;
    public List<SpellHolder> Variants;
    [System.Serializable]
    public class SpellHolder
    {
        public RestrictionSystem.CurrentLearn spell;
        public List<GameObject> Levels;
    }

    public string SpellName(RestrictionSystem.CurrentLearn spell, int Level) { return Spells[(int)spell].Levels[Level].name; }
}
