using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridUIElement : MonoBehaviour {

    CanvasGroup canvasGroup;

	// Use this for initialization
	void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
	}
	
    public void ActivateCanvasGroup()
    {
        canvasGroup.DOFade(1, 0.4f).OnComplete(() => canvasGroup.interactable = true);
    }
}
