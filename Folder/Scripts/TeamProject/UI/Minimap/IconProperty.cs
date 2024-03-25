using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconProperty : MonoBehaviour
{
    RectTransform _rect = null;
    [SerializeField] protected Transform myTarget = null;

    protected RectTransform myRect
    {
        get
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }
            return _rect;
        }
    }

    Image _img = null;
    protected Image myImage
    {
        get
        {
            if (_img == null)
            {
                _img = GetComponent<Image>();
                if (_img == null)
                {
                    _img = GetComponentInChildren<Image>();
                }
            }
            return _img;
        }                       
    }
    Image[] _allImgs = null;
    protected Image[] allImages
    {
        get
        {
            if (_allImgs == null)
            {
                _allImgs = GetComponentsInChildren<Image>();
            }
            return _allImgs;
        }
    }

    protected Image mainImage
    {
        get => allImages[allImages.Length - 1];
    }
}
