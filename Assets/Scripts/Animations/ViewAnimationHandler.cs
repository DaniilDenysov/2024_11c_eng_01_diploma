using UnityEngine;
using DG.Tweening;

public class ViewAnimationHandler : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float initialScale = 0.9f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float finalScale = 0.8f;
    [SerializeField] private float bounceIntensity = 2f;
    private void OnEnable()
    {
        transform.localScale = Vector3.one * initialScale;

        transform.DOScale(endScale, animationDuration)
        .SetEase(Ease.OutBack, bounceIntensity)
        .From(Vector3.one * initialScale);
    }

    public void OpenWindow()
    {
        gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        transform.DOScale(finalScale, animationDuration)
            .SetEase(Ease.InBack, bounceIntensity)
            .From(Vector3.one * endScale)
            .OnComplete(() => gameObject.transform.parent.gameObject.SetActive(false));
    }

    public bool IsOpened() => gameObject.transform.parent.gameObject.activeInHierarchy;
}
