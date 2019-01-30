namespace UnityStandardAssets.CinematicEffects
{
    using UnityEngine;

    public sealed class AE_MinAttribute : PropertyAttribute
    {
        public readonly float min;

        public AE_MinAttribute(float min)
        {
            this.min = min;
        }
    }
}
