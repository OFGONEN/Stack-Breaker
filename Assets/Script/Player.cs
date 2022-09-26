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

// Private
	float jump_speed_cofactor = 1f;

    UnityMessage onUpdateMethod;
    UnityMessage onFixedUpdateMethod;
    UnityMessage onInputFingerDown;
    UnityMessage onInputFingerUp;

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
		onInputFingerUp     = ExtensionMethods.EmptyMethod;

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
		onUpdateMethod = RotateAroundOrigin;

		// Activate input via delay
		DOVirtual.DelayedCall( GameSettings.Instance.player_input_activation_delay, () => onInputFingerDown = Jump );
	}

	public void OnInputFingerDown()
	{
		onInputFingerDown();
	}

	public void OnInputFingerUp()
	{
		onInputFingerUp();
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

	void OnJumpComplete()
	{
		onUpdateMethod = ExtensionMethods.EmptyMethod;
		jump_speed_cofactor = 1f;
	}

	void RotateAroundOrigin()
	{
		var position    = transform.position;
		var rotatePoint = Vector3.up * position.y;

		transform.RotateAround( rotatePoint, Vector3.up, Time.deltaTime * GameSettings.Instance.player_rotation_speed * jump_speed_cofactor );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}