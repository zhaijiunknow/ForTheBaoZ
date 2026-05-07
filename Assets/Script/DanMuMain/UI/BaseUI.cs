using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    private Transform _centerTrans = null;
    private Transform _background;
    private Image _backgroundImg;
        
    private readonly float _popScale = 1.2f;
    private readonly float _hideDuration = 0.3f;
    private readonly float _showDuration = 0.5f;
    
    public virtual void OnEnable()
    {
        _centerTrans = transform.Find("Center");
        _background = transform.Find("Bg");
        if (_background != null)
        {
            _backgroundImg = _background.GetComponent<Image>();
        }
    }
    
    public virtual void Show(object ojs = null)
    {
        //Ac.PlaySound(Ac.openPopup);
        gameObject.SetActive(true);
        if (_centerTrans != null)
        {
            if (_backgroundImg != null)
            {
                _backgroundImg.color = new Color(0, 0, 0, 0);
                _backgroundImg.DOColor(new Color(0, 0, 0, 0.7f), _showDuration);
            }
            _centerTrans.localScale = Vector3.zero;
            Sequence popupSequence = DOTween.Sequence();
            popupSequence.Append(_centerTrans.DOScale(_popScale * Vector3.one, 0.5f));
            popupSequence.Append(_centerTrans.DOScale(Vector3.one, 0.3f));
            popupSequence.SetEase(Ease.OutBack);
        }
    }

    public virtual void Hide()
    {
        //Ac.PlaySound(Ac.closePopup);
        if (_centerTrans != null)
        {
            if (_backgroundImg != null)
            {
                _backgroundImg.DOColor(new Color(0, 0, 0, 0), _hideDuration);
            }
            _centerTrans.DOScale(Vector3.zero, _hideDuration / 2)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
