using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Highskillz.Core.AR
{
    public class UIFollowCamera : MonoBehaviour
    {
        public float distance;
        public float followThrsholdDistance = 0.2f;
        private Transform mainCamera;
        private float smooth = 0.1f;

        private Vector3 lastPosition = Vector3.zero;

        void Start()
        {
            this.mainCamera = Camera.main.transform;
            //this.distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        }

        void Update()
        {
            Vector3 futurePos = this.mainCamera.position + this.mainCamera.transform.forward * this.distance;
            float distance = Vector3.Distance(futurePos, this.transform.position);

            if (distance >= followThrsholdDistance)
                lastPosition = futurePos;

            Vector3 vel = Vector3.zero;
            this.transform.position = Vector3.SmoothDamp(this.transform.position, lastPosition, ref vel, smooth, 50f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.mainCamera.transform.rotation, smooth);
        }
    }
}
