/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Stack : MonoBehaviour
{
#region Fields

	static int SHADER_ID_COLOR = Shader.PropertyToID( "_BaseColor" );

	MaterialPropertyBlock propertyBlock;
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
			SetGroundStackColor();
		else
			SetStackColor();
	}
#endregion

#region API
#endregion

#region Implementation
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
		var color    = CurrentLevelData.Instance.levelData.ground_color;

		if( child.gameObject.layer == ExtensionMethods.Layer_Break )
        {
			renderer.material = GameSettings.Instance.stack_break_material;
			color             = CurrentLevelData.Instance.levelData.break_color;
		}

		renderer.GetPropertyBlock( propertyBlock );

		propertyBlock.SetColor( SHADER_ID_COLOR, color );
		renderer.SetPropertyBlock( propertyBlock );
	}

	void SetGroundStackPieceColor( Transform child )
	{
		propertyBlock.Clear();

		var renderer = child.GetComponentInChildren< Renderer >();

		renderer.material = GameSettings.Instance.stack_ground_final_material;
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
