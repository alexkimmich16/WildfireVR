using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

class DirToRot : VFXSpawnerCallbacks
{
    private static readonly int PositionPropertyId = Shader.PropertyToID("ObjectPositionWS");

    private static readonly int PositionAttributeId = Shader.PropertyToID("position");
    private static readonly int OldPositionAttributeId = Shader.PropertyToID("oldPosition");
    private static readonly int VelocityAttributeId = Shader.PropertyToID("velocity");

    public class InputProperties
    {
        public Vector3 ObjectPositionWS = Vector3.zero;
    }

    private float3 _lastPosition;

    public override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {
        _lastPosition = vfxValues.GetVector3(PositionPropertyId);
    }

    public override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {
        if (!state.playing || state.deltaTime == 0) return;

        float3 position = vfxValues.GetVector3(PositionPropertyId);

        state.vfxEventAttribute.SetVector3(OldPositionAttributeId, _lastPosition);
        state.vfxEventAttribute.SetVector3(PositionAttributeId, position);

        state.vfxEventAttribute.SetVector3(VelocityAttributeId, (position - _lastPosition) / state.deltaTime);

        _lastPosition = position;
    }

    public override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {

    }
}
