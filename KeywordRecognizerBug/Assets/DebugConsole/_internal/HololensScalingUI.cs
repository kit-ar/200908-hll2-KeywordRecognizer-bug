using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Highskillz.Core.AR
{
    public class HololensScalingUI : MonoBehaviour
    {

        void Awake()
        {
            // Nice hack bro
#if UNITY_EDITOR
        float zDistance = this.transform.position.z;
        float scale = zDistance * 0.016f;
        this.transform.localScale = new Vector3(scale, scale, 1);

#else
            float zDistance = this.transform.position.z;
            float scale = zDistance * 0.00415f;
            this.transform.localScale = new Vector3(scale, scale, 1);
#endif
        }
    }
}
