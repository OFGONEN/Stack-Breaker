/* Created by and for usage of FF Studios (2021). */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using UnityEditor;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "tool_creator", menuName = "FFEditor/Creator Tool" ) ]
public class CreatorTool : ScriptableObject
{
#region Fields
    [ FoldoutGroup( "Level Create" ), SerializeField ] int level_stack_count;
    [ FoldoutGroup( "Level Create" ), SerializeField ] float level_stack_buffer;
    [ FoldoutGroup( "Level Create" ), SerializeField ] float level_cylinder_scale_cofactor;
    [ FoldoutGroup( "Level Create" ), SerializeField ] float level_player_offset;
    [ FoldoutGroup( "Level Create" ), SerializeField ] StackData level_stack_data;

    [ FoldoutGroup( "Place Stack Piece" ), SerializeField ] Transform place_stack_parent;
    [ FoldoutGroup( "Place Stack Piece" ), SerializeField ] PlaceStackData place_stack_data;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
    [ Button() ]
    void CreateLevel()
    {
		var stackParent = GameObject.Find( "stack_parent" ).transform;
		var cylinder    = GameObject.Find( "gfx_cylinder" ).transform;
		var player      = GameObject.FindWithTag( "Player" ).transform;

		stackParent.DestroyAllChildren();

		float verticalPosition  = 0;
		float lastStackPosition = 0;

		for( var i = 0; i <= level_stack_count; i++ )
        {
			var stack = PrefabUtility.InstantiatePrefab( level_stack_data.stack_gameObject ) as GameObject;
			stack.transform.SetParent( stackParent );
			stack.transform.localPosition = Vector3.up * verticalPosition;

			lastStackPosition  = verticalPosition;
			verticalPosition  += level_stack_buffer + level_stack_data.stack_height;
		}

		cylinder.localScale = Vector3.one.SetY( verticalPosition * level_cylinder_scale_cofactor );
		player.position     = ( Vector3.up * lastStackPosition ).SetZ( level_player_offset );
		player.forward      = Vector3.right;

		Camera.main.GetComponent< CameraFollow >().ResetPosition();
	}

    [ Button() ]
    void PlaceStackPieces() 
    {
        for( var i = 0; i < place_stack_data.stack_count; i++ )
        {
			var piece = PrefabUtility.InstantiatePrefab( place_stack_data.stack_gameObject ) as GameObject;

			piece.transform.SetParent( place_stack_parent );
			piece.transform.localPosition    = Vector3.zero;
			piece.transform.localEulerAngles = Vector3.up * place_stack_data.stack_angle * i;
		}
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}

[ Serializable ]
public struct StackData
{
    public GameObject stack_gameObject;
	public float stack_height;
}

[ Serializable ]
public struct PlaceStackData
{
    public GameObject stack_gameObject;
	public int stack_count;
	public float stack_angle;
}