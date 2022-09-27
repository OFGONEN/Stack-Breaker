/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
#region Fields
  [ Title( "Shared" ) ]
	[ SerializeField ] SharedBoolNotifier notif_input_finger_isPressing;
	[ SerializeField ] SharedFloatNotifier notif_player_width;
  [ Title( "Fired Events" ) ]
	[ SerializeField ] GameEvent event_level_failed;

  [ Title( "Components" ) ]
    [ SerializeField ] Rigidbody _rigidBody;
    [ SerializeField ] Collider _collider;

// Private
	float jump_speed_cofactor = 1f;
	float current_position    = 0;
	bool  jump_collided_break = false;

    UnityMessage onUpdateMethod;
    UnityMessage onFixedUpdateMethod;
    UnityMessage onInputFingerDown;

	RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onInputFingerDown   = ExtensionMethods.EmptyMethod;

		_collider.enabled = false;

		notif_player_width.SetValue_NotifyAlways( CurrentLevelData.Instance.levelData.player_width_ratio );

		OnLevelStartMethod();
	}

    private void Update()
    {
		onUpdateMethod();
	}

    private void FixedUpdate()
    {
		onFixedUpdateMethod();
	}
#endregion

#region API
    public void OnLevelStartMethod()
    {
		current_position = transform.position.y;
		StartMovement();
	}

	public void OnInputFingerDown()
	{
		onInputFingerDown();
	}

	public void OnPlayerGainedWeight( IntGameEvent gameEvent )
	{
		IncreasePlayerWidth( gameEvent.eventValue );
	}

	public void OnTrigger_Ground()
	{
		if( notif_input_finger_isPressing.sharedValue && !jump_collided_break )
		{
			DecreasePlayerWidth( 1 );

			if( notif_player_width.sharedValue > 0 )
				StartMovement();
			else
				event_level_failed.Raise();
		}
		else
			StartMovement();

		// FFLogger.PopUpText( transform.position + Vector3.up, "Ground Trigger" );
	}

	public void OnTrigger_Break( Collider collider )
	{
		// FFLogger.PopUpText( transform.position + Vector3.up, "Break Trigger" );
		if( notif_input_finger_isPressing.sharedValue )
		{
			DecreasePlayerWidth( 1 );

			if( notif_player_width.sharedValue <= 0 )
				event_level_failed.Raise();
			else
			{
				current_position -= GameSettings.Instance.player_step_height;
				//todo collider.GetComponent< Break >.Break();
			}
		}
		else
			StartMovement();
	}
#endregion

#region Implementation
	void IncreasePlayerWidth( int value )
	{
		notif_player_width.SharedValue += value;
	}

	void DecreasePlayerWidth( int value )
	{
		notif_player_width.SharedValue -= value;
	}

	[ Button() ]
	void Jump()
	{
		onInputFingerDown = ExtensionMethods.EmptyMethod;

		jump_collided_break = false;
		jump_speed_cofactor = GameSettings.Instance.player_movement_rotate_cofactor_jumping;

		recycledTween.Recycle( transform.DOMoveY( GameSettings.Instance.player_jump_height,
			GameSettings.Instance.player_jump_duration )
			.SetRelative()
			.SetEase( GameSettings.Instance.player_jump_ease ),
			OnJumpComplete
		);
	}

	void StartMovement()
	{
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;  // Stop falling down no matter what
		onUpdateMethod      = RotateAroundOrigin; // Contiunue movement on a stack

		_collider.enabled  = false;
		transform.position = transform.position.SetY( current_position );

		DOVirtual.DelayedCall( GameSettings.Instance.player_input_activation_delay, SetFingerDownToJump );
	}

	void FallDown()
	{
		var position = transform.position;
		_rigidBody.MovePosition( position + Vector3.down * GameSettings.Instance.player_fall_speed * Time.fixedDeltaTime );
	}

	void RotateAroundOrigin()
	{
		var position    = transform.position;
		var rotatePoint = Vector3.up * position.y;

		transform.RotateAround( rotatePoint, Vector3.up, Time.deltaTime * GameSettings.Instance.player_rotation_speed * jump_speed_cofactor );
	}

	void OnJumpComplete()
	{
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = FallDown;

		jump_speed_cofactor = 1f;
		_collider.enabled   = true;
	}

	void SetFingerDownToJump()
	{
		onInputFingerDown = Jump;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}