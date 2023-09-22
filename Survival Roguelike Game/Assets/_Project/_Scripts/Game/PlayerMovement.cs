using System;
using UnityEngine;

namespace _Project._Scripts.Game
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Fields")]
		[SerializeField] private float moveSpeed;

		private float _horizontal;
		private float _vertical;

		private Transform _thisTransform;

		private void Awake()
		{
			_thisTransform = transform;
		}

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
			_thisTransform.position +=
				new Vector3(_horizontal, 0f, _vertical).normalized * (moveSpeed * Time.deltaTime);
		}
	}
}
