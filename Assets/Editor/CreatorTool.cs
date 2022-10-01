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
    [ FoldoutGroup( "Level Configure" ), SerializeField ] CollectablePatternData[] collectable_pattern_data;
    [ FoldoutGroup( "Level Configure" ), SerializeField, Range( 0f, 1f ) ] float level_hard;

    [ FoldoutGroup( "Stack Pattern" ), SerializeField ] int stack_pattern_count;
    [ FoldoutGroup( "Stack Pattern" ), SerializeField ] int[] stack_pattern_1;
    [ FoldoutGroup( "Stack Pattern" ), SerializeField ] int[] stack_pattern_2;
    [ FoldoutGroup( "Stack Pattern" ), SerializeField ] int[] stack_pattern_3;
    [ FoldoutGroup( "Stack Pattern" ), SerializeField ] int[] stack_pattern_4;

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
		var colliders = stackGround.GetComponentsInChildren< Collider >();

		for( var i = 0; i < renderers.Length; i++ )
			renderers[ i ].material = GameSettings.Instance.stack_ground_final_material;

		// for( var i = 0; i < colliders.Length; i++ )
		// 	colliders[ i ].enabled = false;

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
	void ConfigurePattern()
	{
		var stackParent = GameObject.Find( "parent_stack" ).transform;

		for( var i = 1; i < stackParent.childCount; i++ )
		{
			var stack = stackParent.GetChild( i );
			SetStackPattern( stack, ReturnRandomPatternArray() );
		}
	}

	[ Button() ]
	void ConfigureCollectablePattern()
	{
		EditorSceneManager.MarkAllScenesDirty();
		var collectableParent = GameObject.Find( "parent_collectable" ).transform;
		collectableParent.DestroyAllChildren();

		var stackParent = GameObject.Find( "parent_stack" ).transform;

		for( var i = 1; i < stackParent.childCount; i++ )
		{
			var random = UnityEngine.Random.Range( 0f, 1f );

			if( random >= level_hard )
				PlaceRandomCollectablePatternAtIndex( i );
		}


		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	void PlaceRandomCollectablePatternAtIndex( int index )
	{
		EditorSceneManager.MarkAllScenesDirty();
		var collectableParent = GameObject.Find( "parent_collectable" ).transform;
		var placeHeight = index * level_stack_buffer + collectable_data.collectable_step_height;

		var randomPattern = collectable_pattern_data.ReturnRandom().collectable_angle_array;

		var randomAngle = UnityEngine.Random.Range( 0f, 360f );

		for( var i = 0; i < randomPattern.Length; i++ )
		{
			var collectable = PrefabUtility.InstantiatePrefab( collectable_data.collectable_gameObject ) as GameObject;
			collectable.transform.position = Vector3.up * placeHeight + Vector3.right * collectable_data.collectable_place_offset;
			collectable.transform.SetParent( collectableParent );

			collectable.transform.RotateAround( Vector3.up * placeHeight, Vector3.up, randomPattern[ i ] + randomAngle );
		}
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

	[ MenuItem( "FFGame/Set Pieces As Break %&b" ) ]
	static void SetPiecesAsBreak()
	{
		EditorSceneManager.MarkAllScenesDirty();

		var selection = Selection.gameObjects;

		for( var i = 0; i < selection.Length; i++ )
		{
			var gameObject = selection[ i ];
			// gameObject.layer = ExtensionMethods.Layer_Break;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Break );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_break_material;
		}

		AssetDatabase.SaveAssets();
	}

	[ MenuItem( "FFGame/Set Pieces As Ground %&g" ) ]
	static void SetPiecesAsGround()
	{
		EditorSceneManager.MarkAllScenesDirty();

		var selection = Selection.gameObjects;

		for( var i = 0; i < selection.Length; i++ )
		{
			var gameObject = selection[ i ];
			// gameObject.layer = ExtensionMethods.Layer_Ground;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Ground );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_ground_material;
		}

		AssetDatabase.SaveAssets();
	}

	[ MenuItem( "FFGame/Stack Pattern 1 %#1" ) ]
	static void SetStackPattern_One()
	{
		var creatorTool = AssetDatabase.LoadAssetAtPath( "Assets/Editor/tool_creator.asset", typeof( CreatorTool ) ) as CreatorTool;
		SetStackPattern( creatorTool.stack_pattern_1 );
	}

	[ MenuItem( "FFGame/Stack Pattern 2 %#2" ) ]
	static void SetStackPattern_Two()
	{
		var creatorTool = AssetDatabase.LoadAssetAtPath( "Assets/Editor/tool_creator.asset", typeof( CreatorTool ) ) as CreatorTool;
		SetStackPattern( creatorTool.stack_pattern_2 );
	}

	[ MenuItem( "FFGame/Stack Pattern 3 %#3" ) ]
	static void SetStackPattern_Three()
	{
		var creatorTool = AssetDatabase.LoadAssetAtPath( "Assets/Editor/tool_creator.asset", typeof( CreatorTool ) ) as CreatorTool;
		SetStackPattern( creatorTool.stack_pattern_3 );
	}

	[ MenuItem( "FFGame/Stack Pattern 4 %#4" ) ]
	static void SetStackPattern_Four()
	{
		var creatorTool = AssetDatabase.LoadAssetAtPath( "Assets/Editor/tool_creator.asset", typeof( CreatorTool ) ) as CreatorTool;
		SetStackPattern( creatorTool.stack_pattern_4 );
	}

	static void SetStackPattern( int[] array )
	{
		EditorSceneManager.MarkAllScenesDirty();

		var selection = Selection.activeGameObject.transform;

		var stackComponent = selection.GetComponent<Stack>();

		if( stackComponent == null )
		{
			FFLogger.LogError( "Select A Stack Object" );
			return;
		}

		for( var i = 0; i < selection.childCount; i++ )
		{
			var gameObject = selection.GetChild( i ).gameObject;
			// gameObject.layer = ExtensionMethods.Layer_Ground;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Ground );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_ground_material;
		}

		for( var i = 0; i < array.Length; i++ )
		{
			var gameObject = selection.GetChild( array[ i ] ).gameObject;
			// gameObject.layer = ExtensionMethods.Layer_Break;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Break );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_break_material;
		}

		AssetDatabase.SaveAssets();
	}

	static void SetStackPattern( Transform stack, int[] array )
	{
		EditorSceneManager.MarkAllScenesDirty();

		for( var i = 0; i < stack.childCount; i++ )
		{
			var gameObject = stack.GetChild( i ).gameObject;
			// gameObject.layer = ExtensionMethods.Layer_Ground;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Ground );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_ground_material;
		}

		for( var i = 0; i < array.Length; i++ )
		{
			var gameObject = stack.GetChild( array[ i ] ).gameObject;
			// gameObject.layer = ExtensionMethods.Layer_Break;
			gameObject.SetAllChildrenLayer( ExtensionMethods.Layer_Break );
			gameObject.GetComponentInChildren< Renderer >().material = GameSettings.Instance.stack_break_material;
		}

		stack.localEulerAngles = Vector3.up * UnityEngine.Random.Range( 0f, 360f );

		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	void RotateSelection( float angle )
	{
		var selection = Selection.activeGameObject.transform;
		selection.RotateAround( Vector3.up.SetY( selection.position.y ), Vector3.up, angle );
	}

	int[] ReturnRandomPatternArray()
	{
		int random = UnityEngine.Random.Range( 0, stack_pattern_count );

		FFLogger.Log( "Random: " + random );

		switch( random )
		{
			case 0: return stack_pattern_1;
			case 1: return stack_pattern_2;
			case 2: return stack_pattern_3;
			case 3: return stack_pattern_4;
			default: return stack_pattern_1;
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

[ Serializable ]
public struct CollectableData
{
    public GameObject collectable_gameObject;
    public float collectable_place_offset;
	public float collectable_step_height;
	public float collectable_step_angle;
}

[ Serializable ]
public struct CollectablePatternData
{
	public float[] collectable_angle_array;
}