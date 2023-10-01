using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Game.UI
{
	public class OpenClosePanelButton : MonoBehaviour
	{
		[SerializeField] private GameObject panel;
		[SerializeField] private float animationTime;
		[SerializeField] private Vector3 inPosition;
		[SerializeField] private Vector3 outPosition;
		[SerializeField] private Image buttonImage;
		[SerializeField] private Sprite inSprite;
		[SerializeField] private Sprite outSprite;
		
		private bool _open;
		private bool _canBePressed;
		
		private Button _button;

		private void Awake()
		{
			_open = true;
			_canBePressed = true;
			
			_button = GetComponent<Button>();
			
			_button.onClick.AddListener(OpenClosePanel);
		}

		private void OpenClosePanel()
		{
			if (!_canBePressed)
				return;
			
			if (_open)
				LeanTween.moveX(panel, outPosition.x, animationTime).setOnComplete(SetOpen);
			else
				LeanTween.moveX(panel, inPosition.x, animationTime).setOnComplete(SetOpen);
		}

		private void SetOpen()
		{
			_open = !_open;

			buttonImage.sprite = _open ? inSprite : outSprite;

			_canBePressed = true;
		}
	}
}
