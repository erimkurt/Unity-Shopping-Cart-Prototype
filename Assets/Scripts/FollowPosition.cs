using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
		public enum UpdateType
		{
			Update,
			LateUpdate
		}
		public enum SmoothType
		{
			Lerp,
			SmoothDamp, 
		}

		public Transform target;
		public float lerpSpeed = 20f;
		public float smoothDampTime = 0.02f;
		public bool extrapolatePosition = false;
		public UpdateType updateType;
		public SmoothType smoothType;
		public Vector3 localPositionOffset;

		private Transform _tr;
		private Vector3 _currentPosition;
		private Vector3 _refVelocity;
		
		void Awake () 
		{
			if(target == null)
				target = this.transform.parent;

			_tr = transform;
			_currentPosition = transform.position;
		}

		void OnEnable()
		{
			ResetCurrentPosition();
		}

		void Update () {
			if(updateType == UpdateType.LateUpdate)
				return;
			SmoothUpdate();
		}

		void LateUpdate () {
			if(updateType == UpdateType.Update)
				return;
			SmoothUpdate();
		}

		void SmoothUpdate()
		{
			_currentPosition = Smooth (_currentPosition, target.position, lerpSpeed);
			_tr.position = _currentPosition;
		}

		Vector3 Smooth(Vector3 _start, Vector3 _target, float _smoothTime)
		{
			Vector3 _offset = _tr.localToWorldMatrix * localPositionOffset;

			if (extrapolatePosition) {
				Vector3 difference = _target - (_start - _offset);
				_target += difference;
			}
			_target += _offset;

			switch (smoothType)
			{
				case SmoothType.Lerp:
					return Vector3.Lerp (_start, _target, Time.deltaTime * _smoothTime);
				case SmoothType.SmoothDamp:
					return Vector3.SmoothDamp (_start, _target, ref _refVelocity, smoothDampTime);
				default:
					return Vector3.zero;
			}
		}

		public void ResetCurrentPosition()
		{
			Vector3 _offset = _tr.localToWorldMatrix * localPositionOffset;
			_currentPosition = target.position + _offset;
		}
}
