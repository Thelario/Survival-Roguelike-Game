using TMPro;
using TS.DoubleSlider;
using UnityEngine;

namespace _Project._Scripts.Game.UI
{
	public abstract class DoubleSliderValuesText : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] protected DoubleSlider slider;
		[SerializeField] protected TMP_Text valuesText;

		private void Awake()
		{
			slider.OnValueChanged.AddListener(SliderDoubleValueChanged);
		}

		protected abstract void SliderDoubleValueChanged(float min, float max);
	}
}
