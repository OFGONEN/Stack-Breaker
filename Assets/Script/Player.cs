/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
#region Fields

  [ Title( "Components" ) ]
    [ SerializeField ] Rigidbody _rigidBody;

// Private
    UnityMessage onUpdateMethod;
    UnityMessage onFixedUpdateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		onUpdateMethod      = ExtensionMethods.EmptyMethod;
		onFixedUpdateMethod = ExtensionMethods.EmptyMethod;
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
		onUpdateMethod = OnUpdate_Movement;
	}
#endregion

#region Implementation
    void OnUpdate_Movement()
    {
		var position    = transform.position;
		var rotatePoint = Vector3.up * position.y;

		transform.RotateAround( rotatePoint, Vector3.up, Time.deltaTime * GameSettings.Instance.player_movement_rotation_speed );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}