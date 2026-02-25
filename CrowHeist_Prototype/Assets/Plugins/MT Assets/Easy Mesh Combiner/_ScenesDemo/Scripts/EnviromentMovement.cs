using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
    public class EnviromentMovement : MonoBehaviour
    {
        private Vector3 nextPosition = Vector3.zero;
        private Transform thisTransform;

        public Vector3 pos1;
        public Vector3 pos2;

        void Start()
        {
            thisTransform = this.gameObject.GetComponent<Transform>();
            nextPosition = pos1;
        }

        void Update()
        {
            if (Vector3.Distance(thisTransform.position, nextPosition) > 0.5f)
            {
                this.transform.position = Vector3.Lerp(thisTransform.position, nextPosition, 2.0f * Time.deltaTime);
            }
            else
            {
                if (nextPosition == pos1)
                {
                    nextPosition = pos2;
                    return;
                }
                if (nextPosition == pos2)
                {
                    nextPosition = pos1;
                    return;
                }
            }
        }
    }
}