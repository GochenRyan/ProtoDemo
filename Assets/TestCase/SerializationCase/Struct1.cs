using UnityEngine;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Struct1
{
    public int intValue1;
    public long longValue1;
    public string stringValue1;
    public float floatValue1;
}
