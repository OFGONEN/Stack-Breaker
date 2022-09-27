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
	[ SerializeField ] GameEvent event_level_complete;
	[ SerializeField ] GameEvent event_level_failed;

  [ Title( "Components" ) ]
    [ SerializeField ] Transform transform_punch;
    [ SerializeField ] Transform transform_width;
    [ SerializeField ] Rigidbody _rigidBody;
    [ SerializeField ] Collider _collider;

// Private
	float jump_speed_cofactor = 1f;
	float current_position    = 0;
	bool  jump_collided_break = false;
	Vector3 width_vector = new Vector3( 1, 0, 1 );

	UnityMessage onUpdateMethod;
    UnityMessage onFixedUpdateMethod;
    UnityMessage onInputFingerDown;

	RecycledTween recycledTween = new RecycledTween();
	RecycledTween recycledTween_PlayerPunchScale = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
	private void OnDisable()
	{
		recycledTween.Kill();
		recycledTween_PlayerPunchScale.Kill();
	}

    private void Awake()
    {
		EmptyDelegates();

		_collider.enabled = false;
	}

	private void Start()
	{
		notif_player_width.SetValue_NotifyAlways( CurrentLevelData.Instance.levelData.player_width_ratio );
		UpdatePlayerWidth();
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
		IncreasePlayerWidth( gameEvent.eventValue * CurrentLevelData.Instance.levelData.collectable_cofactor );
	}

	public void OnTrigger_Ground()
	{
		if( transform.position.y <= GameSettings.Instance.player_level_complete_buffer )
			LevelComplete();
		else if( notif_input_finger_isPressing.sharedValue && !jump_collided_break )
		{
			DecreasePlayerWidth( CurrentLevelData.Instance.levelData.break_cofactor );

			if( notif_player_width.sharedValue > 0 )
				StartMovement();
			else
				event_level_failed.Raise();
		}
		else
			StartMovement();

		PunchScalePlayer();

		FFLogger.PopUpText( transform.position + Vector3.up, "Ground Trigger" );
	}

	public void OnTrigger_Break( Collider collider )
	{
		if( notif_input_finger_isPressing.sharedValue )
		{
			DecreasePlayerWidth( CurrentLevelData.Instance.levelData.break_cofactor );
			collider.gameObject.SetActive( false );

			if( notif_player_width.sharedValue <= 0 )
				LevelFailed();
			else
			{
				current_position -= GameSettings.Instance.player_step_height;
				//todo collider.GetComponent< Break >.Break();
			}
		}
		else
			StartMovement();

		PunchScalePlayer();

		FFLogger.PopUpText( transform.position + Vector3.up, "Break Trigger" );
	}
#endregion

#region Implementation
	void IncreasePlayerWidth( float value )
	{
		notif_player_width.SharedValue = Mathf.Clamp( notif_player_width.sharedValue + value,
			0f,
			1f
		);

		UpdatePlayerWidth();
	}

	void DecreasePlayerWidth( float value )
	{
		notif_player_width.SharedValue = Mathf.Clamp( notif_player_width.sharedValue - value,
			0f,
			1f
		);

		UpdatePlayerWidth();
	}

	void UpdatePlayerWidth()
	{
		var scale = GameSettings.Instance.player_width_range.ReturnProgress( notif_player_width.sharedValue );
		transform_width.localScale = new Vector3( scale, 1, scale );
	}

	void PunchScalePlayer()
	{
		if( !recycledTween_PlayerPunchScale.IsPlaying() )
			recycledTween_PlayerPunchScale.Recycle( transform_punch.DOPunchScale( 
				GameSettings.Instance.PlayerPunchVector,
				GameSettings.Instance.player_punch_duraion )
				.SetEase( GameSettings.Instance.player_punch_ease )
			 );
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

	void LevelComplete()
	{
		EmptyDelegates();
		_collider.enabled = false;

		// spawn victory pfx
		// Play victory animation

		event_level_complete.Raise();
	}

	void LevelFailed()
	{
		EmptyDelegates();
		_collider.enabled = false;
		// disable gfx object
		// spawn death pfx

		event_level_failed.Raise();
	}

	void EmptyDelegates()
	{
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onInputFingerDown   = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}