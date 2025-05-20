using Serialization;
using UnityEngine;
using ZeroPass;

[SkipSaveFileSerialization]
public class DamageSpawner : RMonoBehaviour
{
    [Header("Settings")]
    public Vector2 spawnOffset = new Vector2(0, 1f);
    public float positionVariance = 0.5f;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        Subscribe((int)GameHashes.DoDamage, OnDoDamage);
    }

    private void OnDoDamage(object data)
    {
        if (data is DamageData damageData)
        {
            SpawnDamage(damageData.num, damageData.isCritical);
        }
    }

    public void SpawnDamage(int damage, bool isCritical)
    {
        Vector3 spawnPos = transform.position +
                          new Vector3(
                              Random.Range(-positionVariance, positionVariance),
                              spawnOffset.y,
                              0
                          );

        DamageTextController textController = DamageTextPool.Instance.GetFromPool();

        textController.transform.position = spawnPos;
        textController.InitDamageText(damage, isCritical);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SpawnDamage(Random.Range(10, 100), Random.value > 0.8f);
    //    }
    //}
}