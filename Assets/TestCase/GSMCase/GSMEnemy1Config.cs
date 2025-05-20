using GAS.Runtime;
using Serialization;
using UnityEngine;

public class GSMEnemy1Config : IEntityConfig
{
    public const string ID = "GSMEnemy1";

    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "Idle", "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = GSMTestRoot.GSMEnemy1RAC;
        
        var asc = gameObject.AddComponent<AbilitySystemComponent>();
        var enemy = UnityEditor.AssetDatabase.LoadAssetAtPath<AbilitySystemComponentPreset>("GAS/Config/AbilitySystemComponentLib/Enemy");
        asc.InitWithPreset(1, enemy);
        asc.AttrSet<AS_CombatEntity>().InitAttack(5);
        asc.AttrSet<AS_CombatEntity>().InitHP(100);
        asc.AttrSet<AS_CombatEntity>().InitPower(50);

        gameObject.AddComponent<DamageSpawner>();
        gameObject.AddComponent<CombatEntity>();
        gameObject.AddComponent<AbilitySMComponent>();
        gameObject.AddComponent<RAnimControllerBase>();

        return gameObject;
    }

    public void OnPrefabInit(GameObject inst)
    {
    }

    public void OnSpawn(GameObject inst)
    {
    }
}
