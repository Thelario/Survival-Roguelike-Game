using _Project._Scripts.Game.Island_Generation;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Game.UI
{
    public class AnimationTimeSlider : MonoBehaviour
    {
        [SerializeField] private float min;
        [SerializeField] private float max;
        [SerializeField] private float initValue;
        [SerializeField] private Slider slider;

        private void Awake()
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = initValue;
            
            slider.onValueChanged.AddListener(ChangeValue);
        }

        private void ChangeValue(float value)
        {
            GridGenerator.Instance.AnimationTime = value;
        }
    }
}