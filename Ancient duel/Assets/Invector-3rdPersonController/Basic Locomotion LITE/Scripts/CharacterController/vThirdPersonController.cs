using UnityEngine;
using System.Collections;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        float cntr;
        protected virtual void Start()
        {
#if !UNITY_EDITOR
                Cursor.visible = false;
#endif
        }

    }
}