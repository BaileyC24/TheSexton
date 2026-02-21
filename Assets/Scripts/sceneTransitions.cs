using UnityEngine;
using System.Collections;

public abstract class sceneTransitions : MonoBehaviour
{
    public abstract IEnumerator AnimateTransitionIn();
    public abstract IEnumerator AnimateTransitionOut();
}
