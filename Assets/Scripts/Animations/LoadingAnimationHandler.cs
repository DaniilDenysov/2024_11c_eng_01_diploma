using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LoadingAnimationHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text display;

    [SerializeField,Range(0,1000)] private float delay = 0.2f;

    void Start()
    {
        AnimateLoadingText();
    }

    private void AnimateLoadingText()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.AppendCallback(() => display.text = "Loading.")
                       .AppendInterval(delay)
                       .AppendCallback(() => display.text = "Loading..")
                       .AppendInterval(delay)
                       .AppendCallback(() => display.text = "Loading...")
                       .AppendInterval(delay)
                       .AppendCallback(() => display.text = "Loading")
                       .AppendInterval(delay);

        loadingSequence.SetLoops(-1);
    }
}
