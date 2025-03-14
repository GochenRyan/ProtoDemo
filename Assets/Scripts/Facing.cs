using Serialization;
using UnityEngine;
using ZeroPass;

[SkipSaveFileSerialization]
public class Facing : RMonoBehaviour
{
    [MyCmpGet]
    private Animator animator;

    private bool facingLeft;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
    }

    public void Face(float targetX)
    {
        Vector3 localPosition = base.transform.GetLocalPosition();
        float x = localPosition.x;
        if (targetX < x)
        {
            facingLeft = true;
            UpdateMirror();
        }
        else if (targetX > x)
        {
            facingLeft = false;
            UpdateMirror();
        }
    }

    public void Face(Vector3 targetX)
    {
        int num = Grid.CellColumn(Grid.PosToCell(base.transform.GetLocalPosition()));
        int num2 = Grid.CellColumn(Grid.PosToCell(targetX));
        if (num > num2)
        {
            facingLeft = true;
            UpdateMirror();
        }
        else if (num2 > num)
        {
            facingLeft = false;
            UpdateMirror();
        }
    }

    private void UpdateMirror()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.flipX = facingLeft;
    }

    public bool GetFacing()
    {
        return facingLeft;
    }

    public void SetFacing(bool mirror_x)
    {
        facingLeft = mirror_x;
        UpdateMirror();
    }

    public int GetFrontCell()
    {
        int cell = Grid.PosToCell(this);
        if (GetFacing())
        {
            return Grid.CellLeft(cell);
        }
        return Grid.CellRight(cell);
    }
}