/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
//using UnityEngine.UI;

public class DamageTextPanel : PanelUIBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    
    #endregion

    #region field
    /// <summary> テキストの数 </summary>
    private int _textNum;
    private List<DamagePopupTextAnimator> _damageTextAnimators = new List<DamagePopupTextAnimator>();

    private int _currentTextNum = 0;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    protected override void Awake()
    {
        _textNum = transform.childCount;
        InitTexts();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //TakeDamage(Random.Range(1, 11));
        }
    }
    #endregion

    #region public function
    public void TakeDamage(Vector3 pos, int damage)
    {
        _damageTextAnimators[_currentTextNum].AdjustTextPos(pos);
        _damageTextAnimators[_currentTextNum].TakeDamage(damage);

        _currentTextNum++;
        if (_currentTextNum >= _textNum) _currentTextNum = 0;
    }

    public void TakeDamage(Vector2 pos, int damage)
    {
        _damageTextAnimators[_currentTextNum].SetPos(pos);
        _damageTextAnimators[_currentTextNum].TakeDamage(damage);

        _currentTextNum++;
        if (_currentTextNum >= _textNum) _currentTextNum = 0;
    }

    public Vector2 GetUILocalPos(Vector3 targetWorldPos)
    {
        return _damageTextAnimators[0].GetUILocalPos(targetWorldPos);
    }
    #endregion

    #region private function
    /// <summary>
    /// リスト等の変数群のセットアップを行う
    /// </summary>
    private void InitTexts()
    {
        // 全テキストを初期化
        for (int i = 0; i < _textNum; i++)
        {
            DamagePopupTextAnimator damagePopupTextAnimator = 
                transform.GetChild(i).gameObject.GetComponent<DamagePopupTextAnimator>();
            _damageTextAnimators.Add(damagePopupTextAnimator);
        }
    }
    #endregion
}
