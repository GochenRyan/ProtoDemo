using Serialization;
using UnityEngine;

public class GSMEnemy1Config : IEntityConfig
{
    public const string ID = "GSMEnemy1";

    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = GSMTestRoot.GSMEnemy1RAC;
        gameObject.AddComponent<CombatEntity>();
        gameObject.AddComponent<AttackSMComponent>();

        return gameObject;
    }

    public void OnPrefabInit(GameObject inst)
    {
    }

    public void OnSpawn(GameObject inst)
    {
    }
}
