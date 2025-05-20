using TMPro;
using DG.Tweening;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro textComponent;

    [Header("Settings")]
    public float floatHeight = 2f;
    public float floatDuration = 0.8f;
    public float fadeDelay = 0.3f;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void InitDamageText(int damage, bool isCritical = false)
    {
        transform.localScale = originalScale;
        textComponent.alpha = 1f;

        textComponent.text = damage.ToString();

        if (isCritical)
        {
            textComponent.color = Color.yellow;
            transform.localScale = originalScale * 1.5f;

            transform.DOShakePosition(0.4f, 0.3f, 10, 90, false, true);
        }
        else
        {
            textComponent.color = Color.red;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.Join(transform.DOMoveY(transform.position.y + floatHeight, floatDuration)
            .SetEase(Ease.OutQuad));

        sequence.Join(textComponent.DOFade(0, floatDuration)
            .SetDelay(fadeDelay));

        if (!isCritical)
        {
            sequence.Join(transform.DOScale(originalScale * 0.8f, floatDuration)
                .SetEase(Ease.OutBack));
        }

        sequence.OnComplete(() => {
            DamageTextPool.Instance.ReturnToPool(this);
        });
    }

    public void ResetState()
    {
        textComponent.alpha = 1f;
        transform.localScale = originalScale;
        textComponent.color = Color.red;
    }
}