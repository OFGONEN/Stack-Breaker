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
	[ SerializeField ] SharedFloatNotifier notif_level_progress;
	[ SerializeField ] SharedIntNotifier notif_player_stack;

  [ Title( "Fired Events" ) ]
	[ SerializeField ] GameEvent event_level_complete;
	[ SerializeField ] GameEvent event_level_failed;
	[ SerializeField ] FloatGameEvent event_player_stack_break;

  [ Title( "Components" ) ]
    [ SerializeField ] Animator _animator;
    [ SerializeField ] Transform transform_punch;
    [ SerializeField ] SkinnedMeshRenderer _skinnedMeshRenderer;
    [ SerializeField ] Rigidbody _rigidBody;
    [ SerializeField ] Collider _collider;
    [ SerializeField ] ParticleSpawner _particleSpawnner;

// Private
	float jump_speed_cofactor = 1f;
	float start_position      = 0;
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
		UpdateLevelProgress();
	}

    private void FixedUpdate()
    {
		onFixedUpdateMethod();
	}
#endregion

#region API
    public void OnLevelStartMethod()
    {
		start_position   = transform.position.y;
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
		PunchScalePlayer_OnCollectable();
	}

	public void OnTrigger_Ground( Collider collider )
	{
		current_position = collider.transform.position.y;

		if( transform.position.y <= GameSettings.Instance.player_level_complete_buffer )
			LevelComplete();
		else if( notif_input_finger_isPressing.sharedValue && !jump_collided_break )
		{
			var supposedWidth = DecreasePlayerWidth( CurrentLevelData.Instance.levelData.break_cofactor );

			if( supposedWidth < 0 )
				LevelFailed();
			else
			{
				_particleSpawnner.Spawn( 1 );
				PunchScalePlayer_OnGround();
				StartMovement();
			}
		}
		else
			StartMovement();


		// FFLogger.PopUpText( transform.position + Vector3.up, "Ground Trigger" );
	}

	public void OnTrigger_Break( Collider collider )
	{
		current_position = collider.transform.position.y;

		if( notif_input_finger_isPressing.sharedValue )
		{
			collider.gameObject.SetActive( false );
			var supposedWidth = DecreasePlayerWidth( CurrentLevelData.Instance.levelData.break_cofactor );

			if( supposedWidth < 0 )
				LevelFailed();
			else
			{
				PunchScalePlayer_OnCollectable();
				var stack = collider.GetComponentInParent< Stack >(); // Stack
				stack.OnBreak();
				event_player_stack_break.Raise( stack.transform.position.y );

				_particleSpawnner.Spawn( 2 ); // Stack Break
			}
		}
		else
			StartMovement();

		// FFLogger.PopUpText( transform.position + Vector3.up, "Break Trigger" );
	}
#endregion

#region Implementation
	void UpdatePlayerStackCount()
	{
		var stack = Mathf.FloorToInt( notif_player_width.sharedValue / CurrentLevelData.Instance.levelData.break_cofactor );
		notif_player_stack.SetValue_NotifyAlways( stack );
	}
	void IncreasePlayerWidth( float value )
	{
		notif_player_width.SharedValue = Mathf.Clamp( notif_player_width.sharedValue + value,
			0f,
			1f
		);

		UpdatePlayerWidth();
	}

	float DecreasePlayerWidth( float value )
	{
		var width = notif_player_width.sharedValue - value;

		notif_player_width.SharedValue = Mathf.Clamp( width,
			0f,
			1f
		);

		UpdatePlayerWidth();

		return width;
	}

	void UpdatePlayerWidth()
	{
		var scale = GameSettings.Instance.player_width_range.ReturnProgress( notif_player_width.sharedValue );

		DOVirtual.EasedValue( GameSettings.Instance.player_width_range.x,
			GameSettings.Instance.player_width_range.y,
			notif_player_width.sharedValue,
			GameSettings.Instance.player_width_curve );

		_skinnedMeshRenderer.SetBlendShapeWeight( 0, scale );
		// _collider.transform.localScale = new Vector3( scale, 1, scale );
		UpdatePlayerStackCount();
	}

	void PunchScalePlayer_OnGround()
	{
		transform_punch.localScale = Vector3.one;
		recycledTween_PlayerPunchScale.Recycle( transform_punch.DOPunchScale(
			GameSettings.Instance.PlayerPunchVector_Ground,
			GameSettings.Instance.player_punch_ground_duraion )
			.SetEase( GameSettings.Instance.player_punch_ground_ease )
		);
	}

	void PunchScalePlayer_OnCollectable()
	{
		transform_punch.localScale = Vector3.one;
		recycledTween_PlayerPunchScale.Recycle( transform_punch.DOPunchScale(
			GameSettings.Instance.PlayerPunchVector_Collectable,
			GameSettings.Instance.player_punch_collectable_duraion )
			.SetEase( GameSettings.Instance.player_punch_collectable_ease )
		);
	}

	[ Button() ]
	void Jump()
	{
		onInputFingerDown = ExtensionMethods.EmptyMethod;

		_animator.SetBool( "run", false );

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

		_animator.SetBool( "run", true );

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

		_animator.SetTrigger( "fall" );

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

		// Play victory animation
		_animator.SetTrigger( "victory" );

		event_level_complete.Raise();
	}

	void LevelFailed()
	{
		EmptyDelegates();
		_collider.enabled = false;

		// disable gfx object
		_skinnedMeshRenderer.enabled = false;
		// FFLogger.PopUpText( transform.position + Vector3.up / 2f, "Level Failed" );
		_particleSpawnner.Spawn( 0 ); 

		event_level_failed.Raise();
	}

	void EmptyDelegates()
	{
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onInputFingerDown   = ExtensionMethods.EmptyMethod;
	}

	void UpdateLevelProgress()
	{
		notif_level_progress.SharedValue = ( start_position - transform.position.y ) / start_position;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}