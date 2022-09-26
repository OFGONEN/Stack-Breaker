/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class CollisionInterface : MonoBehaviour
{
#region Fields
    [ SerializeField ] CollisionRespondData[] collision_respond_data_array;

    Dictionary< int, int > collision_respond_data_dictionary;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		collision_respond_data_dictionary = new Dictionary< int, int >( collision_respond_data_array.Length );

        for( var i = 0; i < collision_respond_data_array.Length; i++ )
			collision_respond_data_dictionary.Add( collision_respond_data_array[ i ].collision_layer, i );
	}
#endregion

#region API
    public void OnCollision( Collider collider )
    {
		int index;

		if( collision_respond_data_dictionary.TryGetValue( collider.gameObject.layer, out index ) )
			collision_respond_data_array[ index ].collision_event.Invoke();
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
