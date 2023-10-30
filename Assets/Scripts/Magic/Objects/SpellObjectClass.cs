using Sirenix.OdinInspector;
using UnityEngine;
public abstract class SpellObjectClass : SerializedMonoBehaviour
{
    [FoldoutGroup("SpellClass")] public bool AddToAmbientVFX;
    //[FoldoutGroup("SpellClass")] public SpellControlClass Test;
    [FoldoutGroup("SpellClass")] public string EffectName;

    //[FoldoutGroup("SpellClass")] public flo;

    public abstract void SetAudio(bool State);


    [FoldoutGroup("SpellClass")] public FMOD.Studio.EventInstance Sound;

    protected virtual void OnEnable()
    {
        
        
        if (AddToAmbientVFX)
            AmbientParticles.AmbientVFX.instance?.Actives.Add(transform);
        if (EffectName != "" && SoundManager.instance != null)
        {
            Sound = SoundManager.instance.CreateSound(EffectName);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Sound, transform);

            SetAudio(true);
            //+= OnVolumeChange;

            Sound.start();
        }
    }



    protected virtual void Update()
    {
        //Debug.Log("up");
        if (EffectName != "" && SoundManager.instance != null)
            OnVolumeChange();
    }
    protected virtual void OnDisable()
    {
        //Debug.Log("dis");
        if (AddToAmbientVFX)
            AmbientParticles.AmbientVFX.instance?.Actives.Remove(transform);
        if(EffectName != "")
            SetAudio(false);

        Sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void OnVolumeChange() { Sound.setVolume(SoundManager.instance.Volume(EffectName)); }
}
