using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Draws a visual effect at active boundaries when the player tries to cross them.
    /// </summary>
    public class SC_BoundaryVisualizer : MonoBehaviour
    {
        [SerializeField] MeshRenderer _visualizerMesh;

        MaterialPropertyBlock mpb;
        float timer = 0;

        Vector3 hitPos, hitNormal;

        private void OnEnable()
        {
            SC_BoundaryManager.onBoundaryHit += OnBoundaryHit;
            mpb = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = 0;
                    _visualizerMesh.gameObject.SetActive(false);
                }
                else
                {
                    Vector3 camPos = SC_VRRig.mainCamera.transform.position;
                    Vector3 pointOnPlane = Vector3.ProjectOnPlane(camPos - hitPos, hitNormal) + hitPos;
                    float distance = (pointOnPlane - camPos).magnitude;

                    _visualizerMesh.transform.position = pointOnPlane - hitNormal * 0.5f;
                    _visualizerMesh.transform.rotation = Quaternion.LookRotation(hitNormal);
                    mpb.SetFloat("_Intensity", Mathf.Clamp01(timer / 2f) * Mathf.Clamp01(1 - distance));
                    _visualizerMesh.SetPropertyBlock(mpb);
                }
            }
        }

        private void OnDisable()
        {
            SC_BoundaryManager.onBoundaryHit -= OnBoundaryHit;
            mpb = null;
        }

        void OnBoundaryHit(SC_Boundary boundary, RaycastHit hitInfo)
        {
            hitPos = hitInfo.point;
            hitNormal = hitInfo.normal;

            timer = 4;
            _visualizerMesh.gameObject.SetActive(true);
            mpb.SetFloat("_Intensity", 1);
            _visualizerMesh.SetPropertyBlock(mpb);
        }
    }
}
