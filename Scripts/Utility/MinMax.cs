/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct MinMax
{
    #region serialize field
    public float min;
    public float max;
    #endregion

    #region field
    
    #endregion

    #region property
    public float Min { get { return min; } }

	public float Max { get { return max; } }

    public float RandomValue { get { return Random.Range(min, max); } }

	public int RandomIntValue { get { return Random.Range((int)min, (int)max + 1); } }
	#endregion


	#region public function
	public MinMax(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float Clamp(float value)
	{
		return Mathf.Clamp(value, min, max);
	}
	#endregion
}

public class MinMaxRangeAttribute : PropertyAttribute
{
	#region serialize field
	public float minLimit;
	public float maxLimit;
	#endregion

	#region public function
	public MinMaxRangeAttribute(float minLimit, float maxLimit)
	{
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
	}
	#endregion
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
	#region field
	const int numWidth = 50;
	const int padding = 5;
	#endregion

	#region property
	MinMaxRangeAttribute minMaxAttribute { get { return (MinMaxRangeAttribute)attribute; } }
	#endregion

	#region public function
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{

		EditorGUI.BeginProperty(position, label, prop);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		Rect minRect = new Rect(position.x, position.y, numWidth, position.height);
		Rect sliderRect = new Rect(minRect.x + minRect.width + padding, position.y, position.width - numWidth*2 - padding*2, position.height);
		Rect maxRect = new Rect(sliderRect.x + sliderRect.width + padding, position.y, numWidth, position.height);

		SerializedProperty minProp = prop.FindPropertyRelative("min");
		SerializedProperty maxProp = prop.FindPropertyRelative("max");

		float min = minProp.floatValue;
		float max = maxProp.floatValue;
		float minLimit = minMaxAttribute.minLimit;
		float maxLimit = minMaxAttribute.maxLimit;

		min = Mathf.Clamp(EditorGUI.FloatField(minRect, min), minLimit, max);
		max = Mathf.Clamp(EditorGUI.FloatField(maxRect, max), min, maxLimit);
		EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, minLimit, maxLimit);

		minProp.floatValue = min;
		maxProp.floatValue = max;

		EditorGUI.EndProperty();
	}
	#endregion
}
#endif
