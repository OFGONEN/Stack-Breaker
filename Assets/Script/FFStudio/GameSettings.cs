/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields (Settings)
    // Info: You can use Title() attribute ONCE for every game-specific group of settings.
    [ Title( "Player" ) ]
        [ LabelText( "Player Step Height" ) ] public float player_step_height = 0.5f;
        [ LabelText( "Player Input Activation Delay" ) ] public float player_input_activation_delay = 0.5f;
        [ LabelText( "Player Width Range" ) ] public Vector2 player_width_range;
        [ LabelText( "Player Ground Punch Vector" ) ] public Vector3 player_punch_ground_vector = new Vector3( 1, 1 ,1 );
        [ LabelText( "Player Ground Punch Power" ) ] public float player_punch_ground_power = 1;
        [ LabelText( "Player Ground Punch Duration" ) ] public float player_punch_ground_duraion = 0.35f;
        [ LabelText( "Player Ground Punch Ease" ) ] public Ease player_punch_ground_ease = Ease.Linear;
        [ LabelText( "Player Collectable Punch Vector" ) ] public Vector3 player_punch_collectable_vector = new Vector3( 1, 0 ,1 );
        [ LabelText( "Player Collectable Punch Power" ) ] public float player_punch_collectable_power = 1;
        [ LabelText( "Player Collectable Punch Duration" ) ] public float player_punch_collectable_duraion = 0.35f;
        [ LabelText( "Player Collectable Punch Ease" ) ] public Ease player_punch_collectable_ease = Ease.Linear;
 
        [ LabelText( "Player Level Complete Buffer" ) ] public float player_level_complete_buffer = 0.01f;

    [ Title( "Player Movement" ) ]
        [ LabelText( "Player Rotation Speed" ) ] public float player_rotation_speed = 1f;
        [ LabelText( "Player Rotation Speed Cofactor While Jumping" ) ] public float player_movement_rotate_cofactor_jumping = 1f;
        [ LabelText( "Player Jump Height" ) ] public float player_jump_height = 1f;
        [ LabelText( "Player Jump Duration" ) ] public float player_jump_duration = 1f;
        [ LabelText( "Player Jump Ease" ) ] public Ease player_jump_ease = Ease.Linear;
        [ LabelText( "Player Fall Speed" ) ] public float player_fall_speed = 1f;

    [ Title( "Camera" ) ]
        [ LabelText( "Camera Follow Offset" ) ] public Vector3 camera_follow_offset;
        [ LabelText( "Camera Look Axis" ) ] public Vector3 camera_look_axis;
        [ LabelText( "Camera Follow Lateral Speed" ) ] public float camera_follow_speed_lateral = 20f;
        [ LabelText( "Camera Follow Vertical Speed" ) ] public float camera_follow_speed_vertical = 10f;
    
    [ Title( "Project Setup", "These settings should not be edited by Level Designer(s).", TitleAlignments.Centered ) ]
        public int maxLevelCount;
        
        // Info: 3 groups below (coming from template project) are foldout by design: They should remain hidden.
		[ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text relative float height"                ) ] public float ui_PopUp_height;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text float duration"                       ) ] public float ui_PopUp_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Random Spawn Area in Screen" ), SuffixLabel( "percentage" ) ] public float ui_particle_spawn_width; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Duration" ), SuffixLabel( "seconds" ) ] public float ui_particle_spawn_duration; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Ease" ) ] public Ease ui_particle_spawn_ease;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Wait Time Before Target" ) ] public float ui_particle_target_waitTime;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Time" ) ] public float ui_particle_target_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Ease" ) ] public Ease ui_particle_target_ease;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Safe Area Base Top Offset" ) ] public int ui_safeArea_offset_top = 88;

    [ Title( "UI Particle" ) ]
		[ LabelText( "Random Spawn Area in Screen Witdh Percentage" ) ] public float uiParticle_spawn_width_percentage = 10;
		[ LabelText( "Spawn Movement Duration" ) ] public float uiParticle_spawn_duration = 0.1f;
		[ LabelText( "Spanwn Movement Ease" ) ] public DG.Tweening.Ease uiParticle_spawn_ease = DG.Tweening.Ease.Linear;
		[ LabelText( "Target Travel Wait Time" ) ] public float uiParticle_target_waitDuration = 0.16f;
		[ LabelText( "Target Travel Duration" ) ] public float uiParticle_target_duration = 0.4f;
		[ LabelText( "Target Travel Duration (REWARD)" ) ] public float uiParticle_target_duration_reward = 0.85f;
		[ LabelText( "Target Travel Ease" ) ] public DG.Tweening.Ease uiParticle_target_ease = DG.Tweening.Ease.Linear;


        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_duration;
#endregion

#region Property
        public Vector3 PlayerPunchVector_Ground => player_punch_ground_vector * player_punch_ground_power;
        public Vector3 PlayerPunchVector_Collectable => player_punch_collectable_vector * player_punch_collectable_power;
#endregion

#region Fields (Singleton Related)
        static GameSettings instance;

        delegate GameSettings ReturnGameSettings();
        static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion

#region Implementation
        static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
