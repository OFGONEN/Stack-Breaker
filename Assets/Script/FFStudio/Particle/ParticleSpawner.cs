/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class ParticleSpawner : MonoBehaviour
	{
#region Fields
		public ParticleData[] particleDatas;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		[ Button() ]
		public void Spawn( int index )
		{
			var data = particleDatas[ index ];

			Vector3 spawnPosition;

			if( data.offset_local )
				spawnPosition = transform.TransformPoint( data.offset );
			else
				spawnPosition = transform.position + data.offset;

			Transform parent = data.parent ? transform : null;
			data.particle_event.Raise( data.alias, spawnPosition, parent, data.size );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			for( var i = 0; i < particleDatas.Length; i++ )
			{
				var data = particleDatas[ i ];

				Vector3 spawnPosition;

				if( data.offset_local )
					spawnPosition = transform.TransformPoint( data.offset );
				else
					spawnPosition = transform.position + data.offset;

				Handles.Label( spawnPosition, "Particle Spawn:" + data.alias );
				Handles.DrawWireCube( spawnPosition, Vector3.one / 4f );
			}
		}
#endif
#endregion
	}
}