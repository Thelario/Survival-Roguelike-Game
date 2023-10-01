using System;
using _Project._Scripts.Game.Island_Generation;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Game.UI
{
	public class AnimateIslandToggle : MonoBehaviour
	{
		[SerializeField] private Toggle animateIslandToggle;

		private void Awake()
		{
			animateIslandToggle.onValueChanged.AddListener(AnimateIsland);
			
			animateIslandToggle.isOn = true;
		}

		private void Start()
		{
			AnimateIsland(true);
		}

		private void AnimateIsland(bool animate)
		{
			GridGenerator.Instance.AnimateWfcSolver = animate;
		}
	}
}
