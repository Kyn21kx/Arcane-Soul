using System;
using UnityEngine;

public class ME_DemoGUI1 : MonoBehaviour
{
    public int Current = 0;
	public AE_PrefabEffects[] Effects;
  
    public Light Sun;
    public ReflectionProbe ReflectionProbe;
    public Light[] NightLights = new Light[0];
    public Texture HUETexture;
    public bool UseMobileVersion;
    public GameObject Character;
    public GameObject Target;
    public Color guiColor = Color.red;
    //public RFX1_DistortionAndBloom RFX1_DistortionAndBloom;

    private int currentNomber;
	private GameObject currentInstance;
	private GUIStyle guiStyleHeader = new GUIStyle();
    private GUIStyle guiStyleHeaderMobile = new GUIStyle();
    float dpiScale;
    private bool isDay;
    private float colorHUE;
    private float startSunIntensity;
    private Quaternion startSunRotation;
    private Color startAmbientLight;
    private float startAmbientIntencity;
    private float startReflectionIntencity;
    private LightShadows startLightShadows;

    [System.Serializable]
    public class AE_PrefabEffects
    {
        public GameObject ShotEffect;
        public GameObject BowEffect;
        public GameObject BuffEffect;
        public bool UseCharacterTarget = true;
        public RuntimeAnimatorController TargetAnimation;
        public RuntimeAnimatorController CustomAnimation;
    }


    void Start () {
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;
        guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = guiColor;
        guiStyleHeaderMobile.fontSize = (int)(17f * dpiScale);

        ChangeCurrent(Current);
     
        startSunIntensity = Sun.intensity;
	    startSunRotation = Sun.transform.rotation;
	    startAmbientLight = RenderSettings.ambientLight;
	    startAmbientIntencity = RenderSettings.ambientIntensity;
	    startReflectionIntencity = RenderSettings.reflectionIntensity;
	    startLightShadows = Sun.shadows;

	    //RFX1_DistortionAndBloom = Camera.main.GetComponent<RFX1_DistortionAndBloom>();

	}

    bool isButtonPressed;


    private void OnGUI()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            isButtonPressed = false;

        if (GUI.Button(new Rect(10*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "PREVIOUS EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(-1);
        }
        if (GUI.Button(new Rect(160*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "NEXT EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(+1);
        }
        var offset = 0f;
        //if (UseMobileVersion)
        //{
            
        //    offset += 50 * dpiScale;
        //    if (GUI.Button(new Rect(10*dpiScale, 63 * dpiScale, 285*dpiScale, 37*dpiScale), "ON / OFF REALISTIC BLOOM") ||
        //        (!isButtonPressed && Input.GetKeyDown(KeyCode.DownArrow)))
        //    {
        //        isUsedMobileBloom = !isUsedMobileBloom;
        //        RFX1_DistortionAndBloom.UseBloom = isUsedMobileBloom;
        //    }
        //    if(!isUsedMobileBloom) guiStyleHeaderMobile.normal.textColor = new Color(0.8f, 0.2f, 0.2f);
        //    else guiStyleHeaderMobile.normal.textColor = new Color(0.1f, 0.6f, 0.1f);
        //    GUI.Label(new Rect(400 * dpiScale, 15 * dpiScale, 100 * dpiScale, 20 * dpiScale), "Bloom is "+ (isUsedMobileBloom?"Enabled":"Disabled"), guiStyleHeaderMobile);
            
        //}
        if (GUI.Button(new Rect(10*dpiScale, 63*dpiScale + offset, 285*dpiScale, 37*dpiScale), "Day / Night") || (!isButtonPressed && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            isButtonPressed = true;
            if (ReflectionProbe != null) ReflectionProbe.RenderProbe();
            Sun.intensity = !isDay ? 0.05f : startSunIntensity;
            Sun.shadows = isDay ? startLightShadows : LightShadows.None;
            foreach (var nightLight in NightLights)
            {
                nightLight.shadows = !isDay ? startLightShadows : LightShadows.None;
            }
            Sun.transform.rotation = isDay ? startSunRotation : Quaternion.Euler(350, 30, 90);
            RenderSettings.ambientLight = !isDay ? new Color(0.1f, 0.1f, 0.1f) : startAmbientLight;
            var lightInten = !UseMobileVersion ? 1 : 0.2f;
            RenderSettings.ambientIntensity = isDay ? startAmbientIntencity : lightInten;
            RenderSettings.reflectionIntensity = isDay ? startReflectionIntencity : 0.2f;
            isDay = !isDay;
        }
      
        GUI.Label(new Rect(400*dpiScale, 15*dpiScale + offset / 2, 100*dpiScale, 20*dpiScale),
            "Effect " + currentNomber +
            "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
        

        GUI.DrawTexture(new Rect(12*dpiScale, 140*dpiScale + offset, 285*dpiScale, 15*dpiScale), HUETexture, ScaleMode.StretchToFill, false, 0);

        float oldColorHUE = colorHUE;
        colorHUE = GUI.HorizontalSlider(new Rect(12*dpiScale, 147*dpiScale + offset, 285*dpiScale, 15*dpiScale), colorHUE, 0, 360);

        if (Mathf.Abs(oldColorHUE - colorHUE) > 0.001)
        {
            AE_ColorHelper.ChangeObjectColorByHUE(currentInstance, colorHUE / 360f);
            //var transformMotion = currentInstance.GetComponentInChildren<AE_TransformMotion>(true);
            //if (transformMotion != null)
            //{
            //    transformMotion.HUE = colorHUE / 360f;
            //    foreach (var collidedInstance in transformMotion.CollidedInstances)
            //    {
            //        if (collidedInstance != null) AE_ColorHelper.ChangeObjectColorByHUE(collidedInstance, colorHUE / 360f);
            //    }
            //}

            var animator = currentInstance.GetComponent<AE_AnimatorEvents>();
            if (animator != null)
            {
                animator.HUE = colorHUE / 360f;
              
                if (animator.Effect1.CurrentInstance != null) AE_ColorHelper.ChangeObjectColorByHUE(animator.Effect1.CurrentInstance, colorHUE / 360f);
                if (animator.Effect2.CurrentInstance != null) AE_ColorHelper.ChangeObjectColorByHUE(animator.Effect2.CurrentInstance, colorHUE / 360f);
                if (animator.Effect3.CurrentInstance != null) AE_ColorHelper.ChangeObjectColorByHUE(animator.Effect3.CurrentInstance, colorHUE / 360f);
                if (animator.Effect4.CurrentInstance != null) AE_ColorHelper.ChangeObjectColorByHUE(animator.Effect4.CurrentInstance, colorHUE / 360f);
            }

            if (UseMobileVersion)
            {
                //var settingColor = currentInstance.GetComponent<AE_EffectSettingColor>();
                //if (settingColor == null) settingColor = currentInstance.AddComponent<AE_EffectSettingColor>();
                //var hsv = AE_ColorHelper.ColorToHSV(settingColor.Color);
                //hsv.H = colorHUE / 360f;
                //settingColor.Color = AE_ColorHelper.HSVToColor(hsv);
            }

        }
    }

    private GameObject instanceShieldProjectile;

    void ChangeCurrent(int delta)
    {
        colorHUE = 0;
        
        currentNomber+=delta;
		if (currentNomber> Effects.Length - 1)
			currentNomber = 0;
		else if (currentNomber < 0)
			currentNomber = Effects.Length - 1;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            RemoveClones();
        }

       // for (int i = 0; i < 10; i++)
        {

            currentInstance = Instantiate(Character);
            //currentInstance.transform.position += new Vector3(0, 0, 3 - i / 1.5f);
            if (Effects[currentNomber].CustomAnimation != null)
            {
                currentInstance.GetComponent<Animator>().runtimeAnimatorController = Effects[currentNomber].CustomAnimation;
            }

            var animEffect = currentInstance.GetComponent<AE_AnimatorEvents>();
            if (animEffect != null)
            {
                //animEffect.Target = Target;
                if (Effects[currentNomber].ShotEffect != null) animEffect.Effect1.Prefab = Effects[currentNomber].ShotEffect;
                if (Effects[currentNomber].BowEffect != null) animEffect.Effect2.Prefab = Effects[currentNomber].BowEffect;
                if (Effects[currentNomber].BuffEffect != null) animEffect.Effect3.Prefab = Effects[currentNomber].BuffEffect;
                //if (Effects[currentNomber].Target != null)
                if (Effects[currentNomber].UseCharacterTarget)
                {
                    animEffect.Effect4.Prefab = Target;
                    animEffect.Effect4.TargetAnimation = Effects[currentNomber].TargetAnimation;
                }
            }

          

            if (UseMobileVersion)
            {
                //CancelInvoke("ReactivateEffect");
                //var transformMotion = currentInstance.GetComponentInChildren<AE_TransformMotion>();
                //if (transformMotion != null)
                //    transformMotion.CollisionEnter += (sender, info) => { Invoke("ReactivateEffect", 3); };

            }
        }
    }

    

    void RemoveClones()
    {
        var allGO = FindObjectsOfType<GameObject>();
        foreach (var go in allGO)
        {
            if(go.name.Contains("(Clone)")) Destroy(go);
        }
    }

    //void ReactivateEffect()
    //{
    //    currentInstance.SetActive(false);
    //    currentInstance.SetActive(true);
    //}
}
