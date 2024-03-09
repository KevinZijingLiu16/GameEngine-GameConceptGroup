namespace AE0672
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraControl : MonoBehaviour
    {


        public Transform target;
        public Vector3 offset = new Vector3(0f, 2f, -5f);
        public float smoothness = 5.0f;

        private Vector3 previousMousePos;

        void LateUpdate()
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, smoothness * Time.deltaTime);

                transform.GetChild(0).transform.localPosition = offset;

                if (Input.GetMouseButton(1))
                {
                    transform.Rotate(new Vector3(0, previousMousePos.x - Input.mousePosition.x, 0) * 0.5f);
                }
            }
            previousMousePos = Input.mousePosition;
        }
    }



}