/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class ParticleEffect : MonoBehaviour
	{
#region Fields
	[ Title( "Setup" ) ]
		public MultipleEventListenerDelegateResponse level_finish_listener;
		public UnityEvent event_particle_spawn;
		public string alias;

		ParticleEffectPool particle_pool;
		ParticleEffectStopped particleEffectStopped;
		ParticleSystem particles;

		Vector3 particle_start_size;
#endregion

#region UnityAPI
		void OnEnable()
		{
			level_finish_listener.OnEnable();
		}

		void OnDisable()
		{
			level_finish_listener.OnDisable();
		}

		void Awake()
		{
			particles = GetComponentInChildren< ParticleSystem >();

			var mainParticle             = particles.main;
			    mainParticle.stopAction  = ParticleSystemStopAction.Callback;
			    mainParticle.playOnAwake = false;

			level_finish_listener.response = ExtensionMethods.EmptyMethod;

			particle_start_size = transform.localScale;
		}

		void OnParticleSystemStopped()
		{
			level_finish_listener.response = ExtensionMethods.EmptyMethod;

			particleEffectStopped( this );
			particle_pool.ReturnEntity( this );
			transform.localScale = Vector3.one;
		}
#endregion

#region API
		public virtual void InitIntoPool( ParticleEffectPool pool, ParticleEffectStopped effectStoppedDelegate )
		{
			particle_pool         = pool;
			particleEffectStopped = effectStoppedDelegate;
		}

		public void PlayParticle( ParticleSpawnEvent particleEvent )
		{
			gameObject.SetActive( true );
			
			transform.position   = particleEvent.particle_spawn_point;
			transform.localScale = particle_start_size * particleEvent.particle_spawn_size;

			if( particleEvent.particle_spawn_parent != null )
			{
				transform.SetParent( particleEvent.particle_spawn_parent );
				level_finish_listener.response = OnParticleSystemStopped;
			}

			event_particle_spawn.Invoke();
			particles.Play();
		}


		public void PlayParticle( Vector3 position, float scale, Transform parent = null )
		{
			gameObject.SetActive( true );

			transform.position = position;
			transform.localScale = particle_start_size * scale;

			if( parent != null )
			{
				transform.SetParent( parent );
				level_finish_listener.response = OnParticleSystemStopped;
			}

			event_particle_spawn.Invoke();
			particles.Play();
		}
#endregion

	}
}