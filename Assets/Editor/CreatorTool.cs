/* Created by and for usage of FF Studios (2021). */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using UnityEditor;
using UnityEditor.SceneManagement;
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

    [ FoldoutGroup( "Place Collectable" ), SerializeField, Min( 1 ) ] int collectable_place_index;
    [ FoldoutGroup( "Place Collectable" ), SerializeField, Min( 1 ) ] int collectable_place_count;
    [ FoldoutGroup( "Place Collectable" ), SerializeField ] float collectable_place_angle;
    [ FoldoutGroup( "Place Collectable" ), SerializeField ] CollectableData collectable_data;

    [ FoldoutGroup( "Place Stack Piece" ), SerializeField ] Transform place_stack_parent;
    [ FoldoutGroup( "Place Stack Piece" ), SerializeField ] PlaceStackData place_stack_data;

	// Private
	[ ShowInInspector, ReadOnly ] List< GameObject > collectable_last = new List< GameObject >();
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
		EditorSceneManager.MarkAllScenesDirty();

		var stackParent = GameObject.Find( "parent_stack" ).transform;
		var cylinder    = GameObject.Find( "cylinder" ).transform;
		var player      = GameObject.FindWithTag( "Player" ).transform;

		stackParent.DestroyAllChildren();

		float verticalPosition  = 0;
		float lastStackPosition = 0;

		var stackGround = PrefabUtility.InstantiatePrefab( level_stack_data.stack_gameObject ) as GameObject;
		stackGround.transform.SetParent( stackParent );
		stackGround.transform.localPosition = Vector3.up * verticalPosition;
		var renderers = stackGround.GetComponentsInChildren< Renderer >();

		for( var i = 0; i < renderers.Length; i++ )
			renderers[ i ].material = GameSettings.Instance.stack_ground_final_material;

		lastStackPosition = verticalPosition;
		verticalPosition += level_stack_buffer;

		for( var i = 1; i <= level_stack_count; i++ )
        {
			var stack = PrefabUtility.InstantiatePrefab( level_stack_data.stack_gameObject ) as GameObject;
			stack.transform.SetParent( stackParent );
			stack.transform.localPosition = Vector3.up * verticalPosition;

			lastStackPosition  = verticalPosition;
			verticalPosition  += level_stack_buffer;
		}

		cylinder.localScale = Vector3.one.SetY( verticalPosition * level_cylinder_scale_cofactor );
		player.position     = ( Vector3.up * lastStackPosition ).SetZ( level_player_offset );
		player.forward      = Vector3.right;

		Camera.main.GetComponent< CameraFollow >().ResetPosition();

		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	void PlaceCollectable()
	{
		EditorSceneManager.MarkAllScenesDirty();
		collectable_last.Clear();

		var collectableParent = GameObject.Find( "parent_collectable" ).transform;
		var placeHeight       = collectable_place_index * level_stack_buffer + collectable_data.collectable_step_height;

		for( var i = 0; i < collectable_place_count; i++ )
		{
			var collectable = PrefabUtility.InstantiatePrefab( collectable_data.collectable_gameObject ) as GameObject;
			collectable.transform.position = Vector3.up * placeHeight + Vector3.right * collectable_data.collectable_place_offset;
			collectable.transform.SetParent( collectableParent );

			collectable.transform.RotateAround( Vector3.up * placeHeight, Vector3.up, collectable_place_angle * ( i + 1 ) );

			collectable_last.Add( collectable );
		}

		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	void DeleteCollectable()
	{
		EditorSceneManager.MarkAllScenesDirty();

		for( var i = 0; i < collectable_last.Count; i++ )
			DestroyImmediate( collectable_last[ i ] );

		collectable_last.Clear();

		AssetDatabase.SaveAssets();
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

	[ Button(), MenuItem( "FFGame/Set Pieces As Break %#g" ) ]
	static void SetPiecesAsBreak()
	{
		EditorSceneManager.MarkAllScenesDirty();

		var selection = Selection.gameObjects;

		for( var i = 0; i < selection.Length; i++ )
		{
			var gameObject = selection[ i ];
			gameObject.layer = ExtensionMethods.Layer_Break;
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_break_material;
		}

		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	void RotateSelection( float angle )
	{
		var selection = Selection.activeGameObject.transform;
		selection.RotateAround( Vector3.up.SetY( selection.position.y ), Vector3.up, angle );
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

[ Serializable ]
public struct CollectableData
{
    public GameObject collectable_gameObject;
    public float collectable_place_offset;
	public float collectable_step_height;
	public float collectable_step_angle;
}