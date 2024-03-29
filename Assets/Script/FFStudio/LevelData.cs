﻿/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.IO;
using System.Collections;
using static UnityEngine.ParticleSystem;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" ) ]
	public class LevelData : ScriptableObject
    {
	[ Title( "Setup" ) ]
		[ ValueDropdown( "SceneList" ), LabelText( "Scene Index" ) ] public int scene_index;
        [ LabelText( "Override As Active Scene" ) ] public bool scene_overrideAsActiveScene;
        [ LabelText( "Player Start Width Ratio" ) ] public float player_width_ratio;
        [ LabelText( "Collectable Cofactor" ) ] public float collectable_cofactor;
        [ LabelText( "Break Cofactor" ) ] public float break_cofactor;
        [ LabelText( "Break Color" ) ] public Color break_color;
        [ LabelText( "Ground Color" ) ] public Color ground_color;
        [ LabelText( "Ground Final Color" ) ] public Color ground_final_color;
        [ LabelText( "Cylinder Color" ) ] public Color cylinder_color;
        [ LabelText( "Particle Stack Break Color" ) ] public MinMaxGradient particle_stack_break_color;

#if UNITY_EDITOR
		static IEnumerable SceneList()
        {
			var list = new ValueDropdownList< int >();

			var scene_count = SceneManager.sceneCountInBuildSettings;

			for( var i = 0; i < scene_count; i++ )
				list.Add( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) + $" ({i})", i );

			return list;
		}
#endif
    }
}
