namespace UnityStandardAssets.CinematicEffects
{
    using UnityEngine;

    public sealed class RFX1_MinAttribute : PropertyAttribute
    {
        public readonly float min;

        public RFX1_MinAttribute(float min)
        {
            this.min = min;
        }
    }
}
