using UnityEngine;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ChildCls2 : BaseCls
{
    [Serialize]
    public float FloatValue1;
}
