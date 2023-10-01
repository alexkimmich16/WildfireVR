using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Container/SpellContainer")]
public class SpellContainer : ScriptableObject
{
    public List<SpellHolder> Spells;
    public GameObject FireballWarmup;
    [System.Serializable]
    public class SpellHolder
    {
        public Spell spell;
        public List<GameObject> Levels;
    }

    public string SpellName(Spell spell, int Level) { return Spells[(int)spell - 1].Levels[Level].name; }
}
