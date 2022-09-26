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
  [ Title( "Components" ) ]
    [ SerializeField ] Rigidbody _rigidBody;
    [ SerializeField ] Collider _collider;

// Private
	float jump_speed_cofactor = 1f;
	float current_position    = 0;

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
		onUpdateMethod   = RotateAroundOrigin;

		ActivateInputWithDelay();
	}

	public void OnInputFingerDown()
	{
		onInputFingerDown();
	}

	public void OnTrigger_Ground()
	{
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
		onUpdateMethod      = RotateAroundOrigin;

		transform.position = transform.position.SetY( current_position );

		ActivateInputWithDelay();
	}

	public void OnTrigger_Break()
	{
		current_position -= GameSettings.Instance.player_step_height;
	}
#endregion

#region Implementation
	[ Button() ]
	void Jump()
	{
		jump_speed_cofactor = GameSettings.Instance.player_movement_rotate_cofactor_jumping;

		recycledTween.Recycle( transform.DOMoveY( GameSettings.Instance.player_jump_height,
			GameSettings.Instance.player_jump_duration )
			.SetRelative()
			.SetEase( GameSettings.Instance.player_jump_ease ),
			OnJumpComplete
		);
	}

	void FallDown()
	{
		var position = transform.position;
		_rigidBody.MovePosition( position + Vector3.down * GameSettings.Instance.player_fall_speed * Time.fixedDeltaTime );
	}

	void OnJumpComplete()
	{
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = FallDown;

		jump_speed_cofactor = 1f;
		_collider.enabled   = true;
	}

	void RotateAroundOrigin()
	{
		var position    = transform.position;
		var rotatePoint = Vector3.up * position.y;

		transform.RotateAround( rotatePoint, Vector3.up, Time.deltaTime * GameSettings.Instance.player_rotation_speed * jump_speed_cofactor );
	}

	void ActivateInputWithDelay()
	{
		DOVirtual.DelayedCall( GameSettings.Instance.player_input_activation_delay, () => onInputFingerDown = Jump );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}