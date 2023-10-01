using _Project._Scripts.Game.Island_Generation;
using UnityEngine;

namespace _Project._Scripts.Game.UI
{
    public class DoubleSliderValuesTextRocks : DoubleSliderValuesText
    {
        protected override void SliderDoubleValueChanged(float min, float max)
        {
            int minInt = Mathf.RoundToInt(min);
            int maxInt = Mathf.RoundToInt(max);

            valuesText.text = $"{minInt}-{maxInt}";

            GridGenerator.Instance.MinAmountOfDirtRocks = minInt;
            GridGenerator.Instance.MaxAmountOfDirtRocks = maxInt;
        }
    }
}