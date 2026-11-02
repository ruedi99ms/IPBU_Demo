using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// A boundary object that can limit the players movement.
    /// Shows an effect, when the player tries to move through the boundary.
    /// Use BoundaryManager to manage multiple boundaries.
    /// Do not change the boundary colliders at runtime.
    /// </summary>
    public class SC_Boundary : MonoBehaviour
    {
        Collider[] colliders;

        Vector3 lastCameraPosition;
        bool initialized = false;

        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        private void OnEnable()
        {
            if (initialized)
                lastCameraPosition = SC_VRRig.mainCamera.transform.position;
        }

        private void Start()
        {
            lastCameraPosition = SC_VRRig.mainCamera.transform.position;
            initialized = true;
        }

        private void LateUpdate()
        {
            Vector3 currentCameraPosition = SC_VRRig.mainCamera.transform.position;
            if (currentCameraPosition == lastCameraPosition)
                return;
            if (RaycastBounds(lastCameraPosition, currentCameraPosition, out RaycastHit hit))
            {
                SC_BoundaryManager.onBoundaryHit?.Invoke(this, hit);
            }

            lastCameraPosition = SC_VRRig.mainCamera.transform.position;
        }

        private void OnDisable()
        {

        }

        bool RaycastBounds(Vector3 from, Vector3 to, out RaycastHit hit)
        {
            hit = new RaycastHit();

            Ray ray;
            float length;
            RaycastHit hitInfo;

            bool hitAny = false;
            foreach (Collider collider in colliders)
            {
                if (to == from)
                    continue;
                ray = new Ray(from, (to - from).normalized);
                ray.origin -= ray.direction * 0.01f;
                length = (to - from).magnitude + 0.01f;
                if (collider.Raycast(ray, out hitInfo, length) && Vector3.Dot(hitInfo.normal, ray.direction) <= 0)
                {
                    Vector3 targetPos = Vector3.ProjectOnPlane(to - hitInfo.point, hitInfo.normal) + hitInfo.point;
                    SC_VRRig.rigTransform.position += targetPos - to;
                    to = SC_VRRig.mainCamera.transform.position;

                    hit = hitInfo;
                    hitAny = true;
                }
            }

            return hitAny;
        }
    }
}
