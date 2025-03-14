using UnityEngine;
using ZeroPass;
using ZeroPass.StateMachine;

public class Grid
{
    public static int WidthInCells = 1;

    public static int PosToCell(StateMachine.Instance smi)
    {
        return PosToCell(smi.transform.GetPosition());
    }

    public static int PosToCell(GameObject go)
    {
        return PosToCell(go.transform.GetPosition());
    }

    public static int PosToCell(RMonoBehaviour cmp)
    {
        return PosToCell(cmp.transform.GetPosition());
    }

    public static int PosToCell(Vector2 pos)
    {
        float x = pos.x;
        float num = pos.y + 0.05f;
        int num2 = (int)num;
        int num3 = (int)x;
        return num2 * WidthInCells + num3;
    }

    public static int PosToCell(Vector3 pos)
    {
        float x = pos.x;
        float num = pos.y + 0.05f;
        int num2 = (int)num;
        int num3 = (int)x;
        return num2 * WidthInCells + num3;
    }

    public static int CellRow(int cell)
    {
        return cell / WidthInCells;
    }

    public static int CellColumn(int cell)
    {
        return cell % WidthInCells;
    }

    public static int CellAbove(int cell)
    {
        return cell + WidthInCells;
    }

    public static int CellBelow(int cell)
    {
        return cell - WidthInCells;
    }

    public static int CellLeft(int cell)
    {
        return (cell % WidthInCells <= 0) ? (-1) : (cell - 1);
    }

    public static int CellRight(int cell)
    {
        return (cell % WidthInCells >= WidthInCells - 1) ? (-1) : (cell + 1);
    }

    public enum SceneLayer
    {
        NoLayer = -2,
        Background = -1,
        Water = 1,
        Ground = 2,
        Grass = 3,
        Entity
    }

    public static float GetLayerZ(SceneLayer layer)
    {
        return 0f;
    }
}