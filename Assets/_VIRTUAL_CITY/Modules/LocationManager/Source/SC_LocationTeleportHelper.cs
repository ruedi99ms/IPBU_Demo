using Numena.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Allows teleporting the player with a fade transition.
    /// </summary>
    public class SC_LocationTeleportHelper : MonoBehaviour
    {
        [SerializeField] float transitionDuration = 2;
        [SerializeField] Color transitionColor = Color.black;
        float timer = 0;

        enum State { Idle, FadeOut, FadeIn }
        State state;

        SC_Location targetLocation;

        public void TeleportToLocation(SC_Location location)
        {
            targetLocation = location;
            switch (state)
            {
                case State.Idle:
                    state = State.FadeOut;
                    timer = 0;
                    break;
                case State.FadeIn:
                    state = State.FadeOut;
                    timer = transitionDuration / 2 - timer;
                    break;
            }
        }

        private void Update()
        {
            switch (state)
            {
                case State.Idle:
                    return;
                case State.FadeOut:
                    timer += Time.deltaTime;
                    Color col = transitionColor;
                    col.a = Mathf.Clamp01(timer / transitionDuration * 2);
                    NScreenFade.color = col;
                    if (timer >= transitionDuration / 2f)
                    {
                        Teleport();
                        return;
                    }
                    break;
                case State.FadeIn:
                    timer += Time.deltaTime;
                    Color fc = transitionColor;
                    fc.a = 1f - Mathf.Clamp01(timer / transitionDuration * 2);
                    NScreenFade.color = fc;
                    if (timer >= transitionDuration / 2f)
                        state = State.Idle;
                    break;
            }
        }

        void Teleport()
        {
            if (targetLocation == null)
                return;

            timer = 0;
            state = State.FadeIn;

            SC_BoundaryManager.active = false;

            Vector3 camOffset = SC_VRRig.rigTransform.InverseTransformPoint(SC_VRRig.mainCamera.transform.position);
            camOffset.y = 0;
            SC_VRRig.rigTransform.rotation = targetLocation.transform.rotation;
            SC_VRRig.rigTransform.position = targetLocation.transform.position;

            SC_BoundaryManager.active = true;

            targetLocation.onTeleport?.Invoke(targetLocation);
        }
    }
}
