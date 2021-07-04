using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
		public enum UpdateType
		{
			Update,
			LateUpdate
		}
		public Transform target;
		public float smoothSpeed = 20f;
		public bool extrapolateRotation = false;
		public UpdateType updateType;

		private Transform _tr;
		private Quaternion _currentRotation;

		void Awake () 
		{
			if(target == null)
				target = this.transform.parent;

			_tr = transform;
			_currentRotation = transform.rotation;
		}

		void OnEnable()
		{
			ResetCurrentRotation();
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
			_currentRotation = Smooth (_currentRotation, target.rotation, smoothSpeed);
			_tr.rotation = _currentRotation;
		}

		Quaternion Smooth(Quaternion _rotation, Quaternion _targetRotation, float _smoothSpeed)
		{
			if (extrapolateRotation && Quaternion.Angle(_rotation, _targetRotation) < 90f) {
				Quaternion difference = _targetRotation * Quaternion.Inverse (_rotation);
				_targetRotation *= difference;
			}

			return Quaternion.Slerp (_rotation, _targetRotation, Time.deltaTime * _smoothSpeed);
		}

		public void ResetCurrentRotation()
		{
			_currentRotation = target.rotation;
		}
}
