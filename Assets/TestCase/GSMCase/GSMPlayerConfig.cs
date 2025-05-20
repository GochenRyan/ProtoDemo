using GAS.Runtime;
using Serialization;
using UnityEngine;

public class GSMPlayerConfig : IEntityConfig
{
    public const string ID = "GSMPlayer";

    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "Idle", "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = GSMTestRoot.GSMPlayerRAC;
        
        var asc = gameObject.AddComponent<AbilitySystemComponent>();
        var minion = UnityEditor.AssetDatabase.LoadAssetAtPath<AbilitySystemComponentPreset>("GAS/Config/AbilitySystemComponentLib/Minion");
        asc.InitWithPreset(1, minion);
        asc.AttrSet<AS_CombatEntity>().InitAttack(10);
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
