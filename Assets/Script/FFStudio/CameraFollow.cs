/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class CameraFollow : MonoBehaviour
	{
#region Fields
		[ Title( "Setup" ) ]
		[ SerializeField ] SharedReferenceNotifier notif_target_follow_reference;
		[ SerializeField ] SharedReferenceNotifier notif_target_look_reference;

		UnityMessage onUpdateMethod;

		Transform target_transform_follow;
		Transform target_transform_look;
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
			target_offset           = GameSettings.Instance.camera_follow_offset;
			target_transform_follow = notif_target_follow_reference.sharedValue as Transform;
			target_transform_look   = notif_target_look_reference.sharedValue as Transform;

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
			transform.LookAtAxis( target_transform_look.position, GameSettings.Instance.camera_look_axis );
		}

		void SetPosition()
		{
			var position = transform.position;
			var targetPosition = target_transform_follow.TransformPoint( target_offset );

			var lateralDelta  = GameSettings.Instance.camera_follow_speed_lateral * Time.deltaTime;
			var verticalDelta = GameSettings.Instance.camera_follow_speed_vertical * Time.deltaTime;

			targetPosition.x = Mathf.Lerp( position.x, targetPosition.x, lateralDelta );
			targetPosition.y = Mathf.Lerp( position.y, targetPosition.y, lateralDelta );
			targetPosition.z = Mathf.Lerp( position.z, targetPosition.z, lateralDelta );

			transform.position = targetPosition;
			// transform.position = Vector3.Lerp( transform.position, targetPosition, Time.deltaTime * GameSettings.Instance.camera_follow_speed_lateral );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
		[ Button() ]
		public void ResetPosition()
		{
			var player = GameObject.FindGameObjectWithTag( "Player" ).transform;

			transform.position = player.TransformPoint( GameSettings.Instance.camera_follow_offset * 0.8f );
			transform.LookAtAxis( player.GetChild( 5 ).position, GameSettings.Instance.camera_look_axis );
		}
#endif
#endregion
	}
}