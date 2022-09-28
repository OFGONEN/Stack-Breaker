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
public struct PlaceStackData
{
    public GameObject stack_gameObject;
	public int stack_count;
	public float stack_angle;
}