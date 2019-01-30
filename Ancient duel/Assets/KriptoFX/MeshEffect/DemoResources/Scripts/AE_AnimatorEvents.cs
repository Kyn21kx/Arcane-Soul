using UnityEngine;
using System.Collections;

public class AE_AnimatorEvents : MonoBehaviour
{
    public AE_EffectAnimatorProperty Effect1;
    public AE_EffectAnimatorProperty Effect2;
    public AE_EffectAnimatorProperty Effect3;
    public AE_EffectAnimatorProperty Effect4;
    public GameObject Bow;
    public GameObject Arrow;

    [HideInInspector] public float HUE = -1;

    [System.Serializable]
    public class AE_EffectAnimatorProperty
    {
        [HideInInspector] public RuntimeAnimatorController TargetAnimation;
        public GameObject Prefab;
        public Transform BonePosition;
        public Transform BoneRotation;
        public float DestroyTime = 10;
        [HideInInspector] public GameObject CurrentInstance;
    }

    void InstantiateEffect(AE_EffectAnimatorProperty effect, bool returnIfCreatedInstance = false)
    {
        if (effect.Prefab == null) return;
       // if (returnIfCreatedInstance && effect.CurrentInstance!= null && GameObject.Find(effect.CurrentInstance.name)) return;

        if (effect.BonePosition != null && effect.BoneRotation != null)
            effect.CurrentInstance = Instantiate(effect.Prefab, effect.BonePosition.position, effect.BoneRotation.rotation);
        else effect.CurrentInstance = Instantiate(effect.Prefab);

        if(effect.TargetAnimation != null)
        {
            effect.CurrentInstance.GetComponent<Animator>().runtimeAnimatorController = effect.TargetAnimation;
        }

        if (Bow != null)
        {
            var setMeshToEffect = effect.CurrentInstance.GetComponent<AE_SetMeshToEffect>();
            if (setMeshToEffect != null && setMeshToEffect.MeshType == AE_SetMeshToEffect.EffectMeshType.Bow)
            {
                setMeshToEffect.Mesh = Bow;
            }
        }

        if (Arrow != null)
        {
            var setMeshToEffect = effect.CurrentInstance.GetComponent<AE_SetMeshToEffect>();
            if (setMeshToEffect != null && setMeshToEffect.MeshType == AE_SetMeshToEffect.EffectMeshType.Arrow)
            {
                setMeshToEffect.Mesh = Arrow;
            }
        }


        if(HUE > -0.9f)
        {
            UpdateColor(effect);
        }

        if (effect.DestroyTime > 0.001f) Destroy(effect.CurrentInstance, effect.DestroyTime);
    }

    public void ActivateEffect1()
    {
        InstantiateEffect(Effect1);
    }

    public void ActivateEffect2()
    {
        InstantiateEffect(Effect2);
    }

    public void ActivateEffect3()
    {
        InstantiateEffect(Effect3, true);
    }

    public void ActivateEffect4()
    {
        InstantiateEffect(Effect4);
    }

    private void UpdateColor(AE_EffectAnimatorProperty effect)
    {
        var settingColor = effect.CurrentInstance.GetComponent<AE_EffectSettingColor>();
        if (settingColor == null) settingColor = effect.CurrentInstance.AddComponent<AE_EffectSettingColor>();
        var hsv = AE_ColorHelper.ColorToHSV(settingColor.Color);
        hsv.H = HUE;
        settingColor.Color = AE_ColorHelper.HSVToColor(hsv);
    }
}
