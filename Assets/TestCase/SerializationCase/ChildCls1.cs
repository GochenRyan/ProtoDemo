using UnityEngine;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ChildCls1 : BaseCls
{
    [Serialize]
    public int IntValue1;
}
