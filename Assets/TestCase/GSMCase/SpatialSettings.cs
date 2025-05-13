using UnityEngine;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptOut)]
public class SpatialSettings
{
    [Range(0, 1)]
    public float spatialBlend = 0.8f;
    public float minDistance = 1f;
    public float maxDistance = 15f;
    public float spread = 45f;
}
