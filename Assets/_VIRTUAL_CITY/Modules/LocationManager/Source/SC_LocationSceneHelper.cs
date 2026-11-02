using Numena.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Numena.SC
{
    /// <summary>
    /// Allows teleporting the player to a different scene with a fade animation.
    /// </summary>
    public class SC_LocationSceneHelper : MonoBehaviour
    {
        [SerializeField] float transitionDuration = 2;
        [SerializeField] Color transitionColor = Color.black;
        float timer = 0;

        enum State { Idle, FadeOut, FadeIn }
        State state;

        static SC_LocationInfo targetLocation = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()//clear to avoid errors
        {
            targetLocation = null;
        }

        public void LoadLocation(SC_LocationInfo location)
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

        private void Start()
        {
            if (targetLocation != null)
            {
                Teleport();
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
                        LoadScene();
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

        void LoadScene()
        {
            SceneManager.LoadScene(targetLocation.scene);
        }

        void Teleport()
        {
            timer = 0;
            state = State.FadeIn;

            SC_Location loc = SC_LocationManager.FindLocationInCurrentScene(targetLocation);
            if (loc == null)
            {
                Debug.LogError("Unable to find location: " + targetLocation.title + " in current scene. Cannot teleport.");
                targetLocation = null;
                return;

            }
            targetLocation = null;

            SC_BoundaryManager.active = false;

            Vector3 camOffset = SC_VRRig.rigTransform.InverseTransformPoint(SC_VRRig.mainCamera.transform.position);
            camOffset.y = 0;
            SC_VRRig.rigTransform.rotation = loc.transform.rotation;
            SC_VRRig.rigTransform.position = loc.transform.position;

            SC_BoundaryManager.active = true;

            loc.onTeleport?.Invoke(loc);
        }
    }
}
