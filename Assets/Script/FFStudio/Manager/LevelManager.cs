/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
        [ Header( "Fired Events" ) ]
        public GameEvent event_level_failed;
        public GameEvent event_level_completed;
        public GameEvent event_level_started;

        [ Header( "Level Releated" ) ]
        public SharedFloatNotifier notif_level_progress;
#endregion

#region UnityAPI
#endregion

#region API
        // Info: Called from Editor.
        public void LevelLoadedResponse()
        {
			notif_level_progress.SetValue_NotifyAlways( 0 );

			var levelData = CurrentLevelData.Instance.levelData;
            // Set Active Scene.
			if( levelData.scene_overrideAsActiveScene )
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 1 ) );
            else
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 0 ) );
		}

        // Info: Called from Editor.
        public void LevelRevealedResponse()
        {
			event_level_started.Raise();
		}

        // Info: Called from Editor.
        public void LevelStartedResponse()
        {

        }
#endregion

#region Implementation
#endregion
    }
}