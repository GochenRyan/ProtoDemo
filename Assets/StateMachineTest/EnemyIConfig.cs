using Serialization;
using UnityEngine;
using ZeroPass.StateMachine;

public class EnemyIConfig : IEntityConfig
{
    public const string ID = "EnemyI";
    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = TestSM.HeroRAC;
        return gameObject;
    }

    public void OnPrefabInit(GameObject inst)
    {
    }

    public void OnSpawn(GameObject inst)
    {
        var animator = inst.GetComponent<Animator>();
        animator.Play("attack");
    }
}
