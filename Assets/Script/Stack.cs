/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Stack : MonoBehaviour
{
#region Fields
	[ SerializeField ] ParticleSpawner _particleSpawner;

	static int SHADER_ID_COLOR = Shader.PropertyToID( "_BaseColor" );

	MaterialPropertyBlock propertyBlock;
	UnityMessage onBreakMethod;

	List< Collider > stack_colliders = new List< Collider >( 12 );
	List< Renderer > ground_renderers = new List< Renderer >( 12 );
	List< Renderer > break_renderers = new List< Renderer >( 12 );
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		propertyBlock = new MaterialPropertyBlock();
	}

    private void Start()
    {
        if( Mathf.Approximately( 0, transform.position.y ) )
		{
			onBreakMethod = ExtensionMethods.EmptyMethod;
			SetGroundStackColor();
		}
		else
		{
			onBreakMethod = Break;
			SetStackColor();
		}
	}
#endregion

#region API
	public void OnBreak()
	{
		onBreakMethod();
	}
#endregion

#region Implementation
	void Break()
	{
		_particleSpawner.Spawn( 0 );

		for( var i = 0; i < ground_renderers.Count; i++ )
			ground_renderers[ i ].enabled = false;

		for( var i = 0; i < break_renderers.Count; i++ )
		{
			break_renderers[ i ].enabled = false;
		}

		for( var i = 0; i < stack_colliders.Count; i++ )
			stack_colliders[ i ].enabled = false;
	}

    void SetGroundStackColor()
    {
		for( var i = 0; i < transform.childCount; i++ )
			SetGroundStackPieceColor( transform.GetChild( i ) );
	}

    void SetStackColor()
    {
        for( var i = 0; i < transform.childCount; i++ )
			SetStackPieceColor( transform.GetChild( i ) );
    }

    void SetStackPieceColor( Transform child )
    {
		propertyBlock.Clear();

		var renderer = child.GetComponentInChildren< Renderer >();
		var collider = child.GetComponentInChildren< Collider >();
		var color    = CurrentLevelData.Instance.levelData.ground_color;

		stack_colliders.Add( collider );

		if( child.gameObject.layer == ExtensionMethods.Layer_Break )
        {
			// renderer.material = GameSettings.Instance.stack_break_material;
			color = CurrentLevelData.Instance.levelData.break_color;
			break_renderers.Add( renderer );
		}
		else
			ground_renderers.Add( renderer );

		renderer.GetPropertyBlock( propertyBlock );

		propertyBlock.SetColor( SHADER_ID_COLOR, color );
		renderer.SetPropertyBlock( propertyBlock );
	}

	void SetGroundStackPieceColor( Transform child )
	{
		propertyBlock.Clear();

		var renderer = child.GetComponentInChildren< Renderer >();

		renderer.GetPropertyBlock( propertyBlock );

		propertyBlock.SetColor( SHADER_ID_COLOR, CurrentLevelData.Instance.levelData.ground_final_color );

		renderer.SetPropertyBlock( propertyBlock );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}