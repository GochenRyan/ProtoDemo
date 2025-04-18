using UnityEngine;
using ZeroPass;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ObjectCase : RMonoBehaviour
{
    [Serialize]
    public object Obj1;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }
}
