/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class CameraFollow : MonoBehaviour
	{
#region Fields
		[ Title( "Setup" ) ]
		[ SerializeField ] SharedReferenceNotifier notif_target_reference;

		UnityMessage onUpdateMethod;

		Transform target_transform;
		Vector3 target_offset;
#endregion

#region Properties
#endregion

#region Unity API
		private void Awake()
		{
			onUpdateMethod = ExtensionMethods.EmptyMethod;
		}

		private void Update()
		{
			onUpdateMethod();
		}
#endregion

#region API
		public void OnLevelStarted()
		{
			target_offset    = GameSettings.Instance.camera_follow_offset;
			target_transform = notif_target_reference.sharedValue as Transform;

			onUpdateMethod = FollowTarget;
		}

		public void OnLevelFinished()
		{
			onUpdateMethod = ExtensionMethods.EmptyMethod;
		}
#endregion

#region Implementation
		void FollowTarget()
		{
			SetPosition();
			transform.LookAtAxis( Vector3.zero, Vector3.up );
		}

		void SetPosition()
		{
			var targetPosition = target_transform.TransformPoint( target_offset );
			transform.position = Vector3.Lerp( transform.position, targetPosition, Time.deltaTime * GameSettings.Instance.camera_follow_speed );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
		[ Button() ]
		void ResetPosition()
		{
			var player = GameObject.FindGameObjectWithTag( "Player" ).transform;

			transform.position = player.TransformPoint( GameSettings.Instance.camera_follow_offset );
			transform.LookAtAxis( Vector3.zero, Vector3.up );
		}
#endif
#endregion
	}
}