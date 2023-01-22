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
        public int Variant;
        public GameObject Online;
        public GameObject Offline;
    }

    public string SpellName(RestrictionSystem.CurrentLearn spell, bool Online) { return Online == true ? Spells[(int)spell].Online.name : Spells[(int)spell].Offline.name; }
    public string SpellNameVariant(RestrictionSystem.CurrentLearn spell, bool Online, int Varient)
    {
        for (int i = 0; i < Variants.Count; i++)
            if (Variants[i].spell == spell && Variants[i].Variant == Varient)
                return Online == true ? Variants[i].Online.name : Variants[i].Offline.name;
        return "";
    }
}
