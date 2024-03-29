using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3Dオブジェクトを（段階的に）（半）透明にする機能を提供する。
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DitherableObjctBehaviour : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    /// <summary>1フレームごとにどれくらいずつ alpha を変化させるか指定する</summary>
    [SerializeField] private float _ditherSpeed = 1.5f;
    #endregion

    #region field
    /// <summary>この alpha にするというターゲットの値</summary>
    float _targetAlpha = 1.0f;
    /// <summary>alpha の初期値</summary>
    float _originalAlpha = 1.0f;
    Material _material;
    List<Renderer> _renderers = new List<Renderer>();

    /// <summary> 子オブジェクトの数 </summary>
    int _childlenCount;
    #endregion

    #region property

    #endregion

    #region Unity function
    void Start()
    {
        _childlenCount = transform.childCount;

        // 当たり判定内の複数のオブジェトを透過させたいとき
        if (_childlenCount > 0)
        {
            for (int i = 0; i < _childlenCount; i++)
            {
                _renderers.Add(transform.GetChild(i).gameObject.GetComponent<Renderer>());
            }

            return;
        }

        // 単一オブジェクトを透過させたい時
        // このオブジェクトのマテリアルを取得しておく
        Renderer r = GetComponent<Renderer>();
        if (r)
        {
            _material = r.material;
        }
    }
    #endregion

    #region public function
    /// <summary>
    /// alpha を初期値に戻す
    /// </summary>
    public void ChangeAlpha2Original()
    {
        ChangeAlpha(_originalAlpha);
    }

    /// <summary>
    /// alpha を変更する
    /// </summary>
    /// <param name="targetAlpha">ターゲットとなる alpha の値</param>
    public void ChangeAlpha(float targetAlpha)
    {
        _targetAlpha = targetAlpha;

        if (_material && _childlenCount == 0)
        {
            StartCoroutine(ChangeAlpha());
        }
        else if (_renderers[_childlenCount - 1].material && _childlenCount >  0)
        {
            StartCoroutine(ChangeAlphas());
        }
    }
    #endregion

    #region private function
    /// <summary>
    /// 単一オブジェクト版。alpha を（徐々に）変更する
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeAlpha()
    {
        float matAlpha = _material.GetFloat("_Alpha");

        if (matAlpha > _targetAlpha)
        {
            while (matAlpha > _targetAlpha)
            {
                matAlpha -= _ditherSpeed * Time.deltaTime;
                if (matAlpha < _targetAlpha) matAlpha = _targetAlpha;
                _material.SetFloat("_Alpha", matAlpha);

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (matAlpha < _targetAlpha)
            {
                matAlpha += _ditherSpeed * Time.deltaTime;
                if (matAlpha > _targetAlpha) matAlpha = _targetAlpha;
                _material.SetFloat("_Alpha", matAlpha);

                yield return new WaitForEndOfFrame();
            }
        }

        //Debug.Log("Alpha = " + matAlpha);
    }

    /// <summary>
    /// 複数オブジェクト一括版。alpha を（徐々に）変更する
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeAlphas()
    {
        float matAlpha = _renderers[0].material.GetFloat("_Alpha");

        if (matAlpha > _targetAlpha)
        {
            while (matAlpha > _targetAlpha)
            {
                UpdateAlphas(-_ditherSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (matAlpha < _targetAlpha)
            {
                UpdateAlphas(_ditherSpeed);
                yield return new WaitForEndOfFrame();
            }
        }

        //Debug.Log("Alpha = " + matAlpha);
    }

    private void UpdateAlphas(float m_step)
    {
        for (int i = 0; i < _childlenCount; i++)
        {
            float matAlpha = _renderers[i].material.GetFloat("_Alpha");

            matAlpha += m_step * Time.deltaTime;

            if (m_step < 0)
            {
                if (matAlpha < _targetAlpha) matAlpha = _targetAlpha;
            }
            else
            {
                if (matAlpha > _targetAlpha) matAlpha = _targetAlpha;
            }

            _renderers[i].material.SetFloat("_Alpha", matAlpha);
        }
    }
    #endregion
}
