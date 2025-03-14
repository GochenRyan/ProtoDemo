using UnityEngine;
using ZeroPass;

public static class RPrefabIDExtensions
{
    public static Tag PrefabID(this Component cmp)
    {
        return cmp.gameObject.PrefabID();
    }

    public static Tag PrefabID(this GameObject go)
    {
        return go.GetComponent<RPrefabID>().PrefabTag;
    }

    public static bool HasTag(this Component cmp, Tag tag)
    {
        return cmp.gameObject.HasTag(tag);
    }

    public static bool HasTag(this GameObject go, Tag tag)
    {
        return go.GetComponent<RPrefabID>().HasTag(tag);
    }

    public static void AddTag(this GameObject go, Tag tag)
    {
        go.GetComponent<RPrefabID>().AddTag(tag, false);
    }

    public static void AddTag(this Component cmp, Tag tag)
    {
        cmp.gameObject.AddTag(tag);
    }

    public static void RemoveTag(this GameObject go, Tag tag)
    {
        go.GetComponent<RPrefabID>().RemoveTag(tag);
    }

    public static void RemoveTag(this Component cmp, Tag tag)
    {
        cmp.gameObject.RemoveTag(tag);
    }
}