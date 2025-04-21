using UnityEngine;
using ZeroPass;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class InheritCase : RMonoBehaviour
{
    [Serialize]
    public BaseCls baseCls;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }
}
