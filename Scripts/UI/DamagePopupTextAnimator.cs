/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopupTextAnimator : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    [SerializeField] private float _textAppearDuration = 0.15f;
    [SerializeField] private float _textDisappearDuration = 0.3f;
    [SerializeField] private float _textJumpHeight = 30f;
    #endregion

    #region field
    /// <summary> このオブジェクトのテキストメッシュ </summary>
    private TextMeshProUGUI _textMeshProUGUI;

    /// <summary> このオブジェクトのRectTransform </summary>
    private RectTransform _targetUI;

    /// <summary> 親のRectTransform </summary>
    private RectTransform _parentUI;

    /// <summary> カメラ </summary>
    private Camera _targetCamera;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.text = string.Empty;
        _textMeshProUGUI.DOFade(0, 0);

        _targetUI = GetComponent<RectTransform>();
        _parentUI = _targetUI.parent.GetComponent<RectTransform>();
        _targetCamera = Camera.main;
    }
    #endregion

    #region public function
    /// <summary>
    /// オブジェクトのワールド座標→UIローカル座標に変換
    /// </summary>
    /// <param name="targetWorldPos"></param>
    /// <returns></returns>
    public Vector2 GetUILocalPos(Vector3 targetWorldPos)
    {
        // オブジェクトのワールド座標→スクリーン座標変換
        Vector3 targetScreenPos = _targetCamera.WorldToScreenPoint(targetWorldPos);

        // スクリーン座標変換→UIローカル座標変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentUI,
            targetScreenPos,
            null,
            out var uiLocalPos
        );

        return uiLocalPos;
    }

    /// <summary>
    /// UIローカル座標でのポジション変更
    /// </summary>
    /// <param name="pos"></param>
    public void SetPos(Vector2 pos)
    {
        // RectTransformのローカル座標を更新
        _targetUI.localPosition = pos;
    }

    /// <summary>
    /// オブジェクトのワールド座標が送られてきた場合、
    /// UIローカル座標に変換し、UIのポジションを変更する
    /// </summary>
    /// <param name="targetWorldPos"></param>
    public void AdjustTextPos(Vector3 targetWorldPos)
    {
        // オブジェクトのワールド座標→UIローカル座標に変換
        Vector2 uiLocalPos = GetUILocalPos(targetWorldPos);
        // RectTransformのローカル座標を更新
        SetPos(uiLocalPos);
    }

    /// <summary>
    /// 敵への与ダメ値アニメーション
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        _textMeshProUGUI.DOFade(0, 0);
        _textMeshProUGUI.text = damage.ToString();

        var tmpAnimator = new DOTweenTMPAnimator(_textMeshProUGUI);

        for (var i = 0; i < tmpAnimator.textInfo.characterCount; i++)
        {
            tmpAnimator.DOScaleChar(i, 0.7f, 0);
            var charOffset = tmpAnimator.GetCharOffset(i);

            var sequence = DOTween.Sequence();

            // 登場
            sequence.Append(tmpAnimator.DOOffsetChar(i, charOffset + new Vector3(0f, _textJumpHeight, 0f), _textAppearDuration)
                    .SetEase(Ease.OutFlash, 2))
                .Join(tmpAnimator.DOFadeChar(i, 1f, _textAppearDuration/2f))
                .Join(tmpAnimator.DOScaleChar(i, 1f, _textAppearDuration)
                    .SetEase(Ease.OutBack))
                .SetDelay(0.05f * i);

            // タイミングを合わせて0.5秒待つ
            sequence.AppendInterval(0.05f * (tmpAnimator.textInfo.characterCount - i))
                .AppendInterval(0.5f);

            // 消滅
            sequence.Append(tmpAnimator.DOFadeChar(i, 0, _textDisappearDuration));
            sequence.Join(tmpAnimator.DOOffsetChar(i, charOffset + new Vector3(0, _textJumpHeight, 0), _textDisappearDuration)
                .SetEase(Ease.Linear));
        }
    }
    #endregion

    #region private function
    
    #endregion
}
