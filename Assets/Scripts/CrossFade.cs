using UnityEngine;
using System.Collections;
using DG.Tweening;



public class CrossFade : sceneTransitions
{

    public CanvasGroup crossFade;

    public override IEnumerator AnimateTransitionIn()
    {
        var tweener = crossFade.DOFade(1, 1f);
        yield return tweener.WaitForCompletion();
    }


    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = crossFade.DOFade(0, 1f);
        yield return tweener.WaitForCompletion();
    }






}
