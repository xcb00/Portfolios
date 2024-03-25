using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperty : MonoBehaviour
{
    protected GameObject icon;
    protected float hp = 0f;
    protected Color originColor = Color.white;
    Collider _collider;
    protected Collider myCollider
    {
        get
        {
            if (_collider == null)
            {
                /*Collider[] colliders = GetComponentsInChildren<Collider>();
                if(colliders.Length<1) colliders = GetComponentsInParent<Collider>();

                foreach (Collider col in colliders)
                    if (col.gameObject.CompareTag("Detect")) { _collider = col; break; }*/
                _collider = GetComponentInParent<Collider>();
                if (_collider == null)
                    _collider = GetComponentInChildren<Collider>();//GetComponentsInParent
            }
            return _collider;
        }
    }

    Animator _animator;
    protected Animator myAnimator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInParent<Animator>();
                if (_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    SkinnedMeshRenderer _renderer;
    protected Material myMaterial
    {
        get
        {

            if (_renderer == null)
            {
                _renderer = GetComponentInParent<SkinnedMeshRenderer>();
                if (_renderer == null)
                    _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            return _renderer.material;
        }
    }

    protected virtual void InitProperty()
    {
        _collider = null;
        _animator = null;
        _renderer = null;
    }

    protected IEnumerator CharacterHitting(float time = 0.2f)
    {
        float timing = 0.0f;
        Color clr = myMaterial.color;
        while (timing < time)
        {
            timing = timing + Time.deltaTime > time ? time : timing + Time.deltaTime;
            myMaterial.color = Color.Lerp(clr, Color.red, timing / time);
            yield return null;
        }
        timing = 0.0f;
        while (timing < time)
        {
            timing = timing + Time.deltaTime > time ? time : timing + Time.deltaTime;
            myMaterial.color = Color.Lerp(Color.red, originColor, timing / time);
            yield return null;
        }
    }

    protected void ShowIcon(Color clr)
    {
        //Instantiate(Resources.Load("UI/MiniMapIcon") as GameObject, ResourceManager.Instance.MinimapTrans).GetComponent<MiniMapIcon>().SetTarget(transform, clr);
       icon = PoolManager.Instance.DeququeObject(PoolType.MinimapIcon);
       icon.transform.position = ResourceManager.Instance.MinimapTrans.position;
       icon.GetComponent<MiniMapIcon>().SetTarget(transform, clr);
    }
}
