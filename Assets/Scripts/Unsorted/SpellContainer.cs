using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Container/SpellContainer")]
public class SpellContainer : ScriptableObject
{
    public List<SpellHolder> Spells;

    [System.Serializable]
    public class SpellHolder
    {
        public Spell spell;
        public GameObject Online;
        public GameObject Offline;
    }

    public string SpellName(Spell spell, bool Online)
    {
        if (Online == true)
            return Spells[(int)spell].Online.name;
        else
            return Spells[(int)spell].Offline.name;
    }
}
