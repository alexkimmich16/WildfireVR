using Sirenix.OdinInspector;
public class SpellObjectClass : SerializedMonoBehaviour
{
    [FoldoutGroup("SpellClass")] public bool AddToAmbientVFX;

    private void OnEnable()
    {
        if (AddToAmbientVFX)
            AmbientParticles.AmbientVFX.instance.Actives.Add(transform);
    }
    private void OnDisable()
    {
        if (AddToAmbientVFX)
            AmbientParticles.AmbientVFX.instance.Actives.Remove(transform);
    }
}
