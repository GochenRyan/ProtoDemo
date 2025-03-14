using ZeroPass;

public class GameTags
{
    public static readonly Tag Medicine = TagManager.Create("Medicine");
    public static TagSet UnitCategories = new TagSet
    {
        Medicine
    };

    public static TagSet DisplayAsUnits = new TagSet(UnitCategories);
    public static readonly Tag Assigned = TagManager.Create("Assigned");
    public static readonly Tag Equipped = TagManager.Create("Equipped");
}