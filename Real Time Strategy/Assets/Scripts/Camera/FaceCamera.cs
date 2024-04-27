using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Cameras
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform mainCameraTransform;
        void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        //use LateUpdate for any camera related code
        void LateUpdate()
        {
            transform.LookAt(transform.position + mainCameraTransform.forward,
                mainCameraTransform.up);
        }
    }
}
