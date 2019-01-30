Pack includes prefabs of main effects + prefabs of collision effects (\Assets\KriptoFX\ArcherEffects\Prefabs). 


Support platforms:

	All platforms (PC/Consoles/VR/Mobiles)
	All effects tested on Oculus Rift CV1 with single and dual mode rendering and work perfect. 

------------------------------------------------------------------------------------------------------------------------------------------
NOTE for PC:
	If you want to use posteffect for PC like in the demo video:

	*) Remove "AE_Bloom.cs" from camera if you used this script before. 
	1) Download unity free posteffects 
	https://assetstore.unity.com/packages/essentials/post-processing-stack-83912
	2) Add "PostProcessingBehaviour.cs" on main Camera.
	3) Set the "PostEffects" profile. (path "Assets\KriptoFX\ArcherEffects\PostEffects.asset")
	4) You should turn on "HDR" on main camera for correct posteffects. 
	If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
	or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "MSAA of post effect". It's faster then default MSAA 

------------------------------------------------------------------------------------------------------------------------------------------
NOTE for MOBILES:
	For correct work on mobiles in your project scene you need:

	1) Add script "ME_DistortionAndBloom.cs" on main camera. It's allow you to see correct distortion, soft particles and physical bloom 
	The mobile bloom posteffect work if mobiles supported HDR textures or supported openGLES 3.0

------------------------------------------------------------------------------------------------------------------------------------------
Using effects:

Simple using (without characters):

	1) Just drag and drop prefab of effect on scene and use that (for example, bufs or projectiles).

Using with characters (bow/arrow particles, projectiles, etc):

	1) You can use "animation events" for instantiating an effects in runtime using an animation. (I use this method in the demo scene)
	https://docs.unity3d.com/Manual/animeditor-AnimationEvents.html
	2) You need set the position and the rotation for an effects. I use hand bone position (or center position of arrow) and hand bone rotation.
	3) Some effects can be apllied to bow/arrow mesh. If some prefabs of effects have the script "AE_SetMeshToEffect", 
	then you need set the bow mesh (if mesh type = bow) or set the arrow mesh (if mesh type = arrow)
	
	The package included demo bow with string. (\Assets\KriptoFX\ArcherEffects\DemoResources\Prefabs) 
	For using, you need set the "hand bone" position for "AE_BowString" and set "InHand" when the character pulls the string.

For using effects in runtime, use follow code:

	"Instantiate(prefabEffect, position, rotation);"

Using projectile collision detection:
	void Start ()
	{
		var tm = GetComponentInChildren<AE_PhysicsMotion>(true);
		if (tm!=null) tm.CollisionEnter += Tm_CollisionEnter;
	}

	private void Tm_CollisionEnter(object sender, AE_PhysicsMotion.AE_CollisionInfo e)
	{
	        Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
	}

------------------------------------------------------------------------------------------------------------------------------------------
Effect modification:


	All effects includes helpers scripts (collision behaviour, light/shader animation etc) for work out of box. 
	Also you can add additional scripts for easy change of base effects settings. Just add follow scripts to prefab of effect.
 
	AE_EffectSettingColor - for change color of effect (uses HUE color). Can be added on any effect.
	AE_EffectSettingProjectile - for change projectile physics parameters (speed, mass, etc). 
	AE_EffectQuality - for effects optimisations. When you use few effects, you can use particles limit in scene. 

