using Serialization;
using UnityEngine;
using ZeroPass;
using Debug = ZeroPass.Debug;

[SkipSaveFileSerialization]
public class RSelectable : RMonoBehaviour
{
    private bool selected;

    public string entityName;

    [SerializeField]
    private bool selectable = true;

    public bool IsSelectable
    {
        get
        {
            return selectable && base.isActiveAndEnabled;
        }
        set
        {
            selectable = value;
        }
    }

    private void ClearHighlight()
    {
        Trigger((int)GameHashes.HighlightObject, false);
        //TODO: ClearHighlight
    }

    private void ApplyHighlight(float highlight)
    {
        Trigger((int)GameHashes.HighlightObject, true);
        //TODO: ApplyHighlight
    }

    public void Select()
    {
        selected = true;
        ClearHighlight();
        ApplyHighlight(0.2f);
        Trigger((int)GameHashes.SelectObject, true);
    }

    public void Unselect()
    {
        if (selected)
        {
            selected = false;
            ClearHighlight();
            Trigger((int)GameHashes.SelectObject, false);
        }
    }

    public void SetName(string name)
    {
        entityName = name;
    }

    public virtual string GetName()
    {
        if (entityName == null || entityName == string.Empty || entityName.Length <= 0)
        {
            Debug.Log("Warning Item has blank name!", base.gameObject);
            return base.name;
        }
        return entityName;
    }
}