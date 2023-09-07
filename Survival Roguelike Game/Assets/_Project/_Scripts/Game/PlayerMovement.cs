using UnityEngine;

namespace _Project._Scripts.Game
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Fields")]
		[SerializeField] private float moveSpeed;
		
		[Header("References")]
		[SerializeField] private Transform thisTransform;

		private float _horizontal;
		private float _vertical;
		
		private void Update()
		{
			GetMoveInput();

			MovePlayer();
		}

		private void GetMoveInput()
		{
			_horizontal = Input.GetAxisRaw("Horizontal");
			_vertical = Input.GetAxisRaw("Vertical");
		}

		private void MovePlayer()
		{
			thisTransform.position +=
				new Vector3(_horizontal, 0f, _vertical).normalized * (moveSpeed * Time.deltaTime);
		}
	}
}
