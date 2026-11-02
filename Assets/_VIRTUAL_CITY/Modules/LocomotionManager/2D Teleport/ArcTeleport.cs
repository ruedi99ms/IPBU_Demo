using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Numena.SC
{

    /// <summary>
    /// Arc teleporter. Teleports VR rig and draws teleportation arc
    /// </summary>
    public class ArcTeleport : MonoBehaviour
    {
        [Tooltip("Controls starting point + direction of arc. Usually an empty transform inside the controller")]
        public Transform arcStart;
        [Tooltip("Speed of virtual object that is thrown to create the arc")]
        public float virtualVelocity = 5;
        [Tooltip("Timestep for raycasting. Bigger timestep means less precision")]
        [Range(0.005f, 1)]
        public float timeStep = 0.02f;
        [Tooltip("Maximum number of timesteps. More means longer arc")]
        [Range(5, 512)]
        public int maxSteps = 64;
        [Tooltip("Cast rays with lower resolution than rendered arc. Higher means less precise physics, but better performance")]
        [Range(1, 10)]
        public int downSample = 2;
        public LayerMask raycastLayers = 1;

        public GraphicsElements graphicsElements;

        public InputActionReference primaryStick, secondaryStick;

        bool primaryPressed;
        bool secondaryLeft, secondaryRight;

        [System.Serializable]
        public class GraphicsElements
        {
            [Tooltip("Line renderer used to draw the arc")]
            public LineRenderer arc;
            [Tooltip("line renderer used to draw the circle (target position)")]
            public LineRenderer circle;
            public float circleRadius = 0.15f;
            public int circleSegments = 24;
            [Tooltip("vertical offset of circle, so that it appears above surface")]
            public float circleOffset = 0.05f;

            public Material activatedMaterial;
            public Material deactivatedMaterial;
        }


        bool showArc = false;
        bool teleportPossible = false;
        Vector3 teleportPoint = Vector3.zero;

        private static ArcTeleport m_Main;


        private void Awake()
        {
            m_Main = this;
            CreateCircle(); // create circle geometry for graphics
        }

        private void Start()
        {
        }

        void OnEnable()
        {
            //Input.Input.DefaultConfig.GetButton(RightButton.name).onDown += OnButtonDown;
            //Input.Input.DefaultConfig.GetButton(RightButton.name).onUp += OnButtonUp;
        }
        void OnDisable()
        {
            //Input.Input.DefaultConfig.GetButton(RightButton.name).onDown -= OnButtonDown;
            //Input.Input.DefaultConfig.GetButton(RightButton.name).onUp -= OnButtonUp;
            showArc = false;
            graphicsElements.arc.gameObject.SetActive(false);
            graphicsElements.circle.gameObject.SetActive(false);
        }

        void Update()
        {
            GetInput();

            if (showArc)
            {
                if (graphicsElements.arc.gameObject.activeSelf == false || graphicsElements.circle.gameObject.activeSelf == false)
                {
                    graphicsElements.arc.gameObject.SetActive(true);
                    graphicsElements.circle.gameObject.SetActive(true);
                }
                ParabolicRaycast();
            }
            else
            {
                if (graphicsElements.arc.gameObject.activeSelf == true || graphicsElements.circle.gameObject.activeSelf == true)
                {
                    graphicsElements.arc.gameObject.SetActive(false);
                    graphicsElements.circle.gameObject.SetActive(false);
                }
            }
        }

        void GetInput()//this is so awkward, why doesn't unitys input system handle this better?
        {
            //Vector2 primary = primaryStick.action.ReadValue<Vector2>();
            //Vector2 secondary = secondaryStick.action.ReadValue<Vector2>();
            //Note: there is a bug in Unitys InputSystem that causes the left controller to not work correctly. this is a workaround.
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 primary);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 secondary);

            if (primaryPressed == false)
            {
                if (primary.y >= 0.5f)
                {
                    primaryPressed = true;
                    OnButtonDown();
                }
            }
            else
            {
                if (primary.magnitude < 0.4f)
                {
                    primaryPressed = false;
                    OnButtonUp();
                }
            }
            if (secondaryLeft == false && secondaryRight == false)
            {
                if (secondary.x >= 0.5f)
                {
                    secondaryRight = true;
                    OnTurnRight();
                }
                else if (secondary.x <= -0.5f)
                {
                    secondaryLeft = true;
                    OnTurnLeft();
                }
            }
            else
            {
                if (secondary.magnitude < 0.4f)
                {
                    secondaryLeft = false;
                    secondaryRight = false;
                }
            }
        }

        /// <summary>
        /// When Touchpad is pressed
        /// </summary>
        private void OnButtonDown()
        {
            showArc = true;
        }

        /// <summary>
        /// when touchpad is released
        /// </summary>
        private void OnButtonUp()
        {
            showArc = false;
            if (teleportPossible)
            {
                //perform teleport
                SC_VRRig.rigTransform.position += teleportPoint - GetGroundedPlayerPosition();
            }
        }

        void OnTurnLeft()
        {
            Turn(-30);
        }

        void OnTurnRight()
        {
            Turn(30);
        }

        void Turn(float degrees)
        {
            Vector3 camPos = SC_VRRig.mainCamera.transform.position;
            SC_VRRig.rigTransform.Rotate(new Vector3(0, 1, 0), degrees, Space.Self);
            SC_VRRig.rigTransform.position += camPos - SC_VRRig.mainCamera.transform.position;
        }

        /// <summary>
        /// Perform a series of raycasts in a parabolic shape, based on virtualvelocity and players down vector
        /// </summary>
        private void ParabolicRaycast()
        {
            teleportPossible = false;

            teleportPoint = GetGroundedPlayerPosition(); //set to current position, to indicate that player will not move

            Vector3 firstPoint = arcStart.position;//start of the arc

            Vector3 velocity = Vector3.Scale(arcStart.forward * virtualVelocity, SC_VRRig.rigTransform.lossyScale); //start velocity of virtual thrown object. Defined by controller orientation, virtualvelocity and scale

            Vector3 down = (-SC_VRRig.rigTransform.up * 9.81f) * SC_VRRig.rigTransform.lossyScale.y; //virtual gravity, that pulls the virtual thrown object down
            float totalStep = 0;//virtual time for physics calculation

            Vector3[] previewPoints = new Vector3[maxSteps];//points used to draw the preview line

            Vector3 lastRaycastPoint = firstPoint;//last point from which a raycast was performed

            Vector3 surfaceNormal = Vector3.up;//normal of the surface that was hit

            //create points for line renderer and raycast based on downsampling value
            for (int ct = 0; ct < maxSteps; ct++)
            {
                totalStep = timeStep * ct;//get total virtual time

                //calculate points. Forward velocity * time + 0.5 * down acceleration * squared time. Physics: s = 0.5 at²; s = v*t
                Vector3 nextPoint = firstPoint + velocity * totalStep + 0.5f * down * Mathf.Pow(totalStep, 2);

                previewPoints[ct] = nextPoint;

                //update arc line renderer - ###########################TODO: move outside loop, since updating this repeatedly might be expensive##############################
                graphicsElements.arc.positionCount = ct + 1;
                graphicsElements.arc.SetPosition(ct, nextPoint);

                //perform raycast only if current point is used for raycast(skip points by downsampling)
                if (ct > 0 && ct % downSample == 0)
                {//raycasting logic
                    Vector3 delta = (nextPoint - lastRaycastPoint);
                    Ray ray = new Ray(lastRaycastPoint, delta.normalized);
                    float raylength = delta.magnitude;

                    RaycastHit hit;

                    bool intersecting = Physics.Raycast(ray, out hit, raylength, raycastLayers, QueryTriggerInteraction.Ignore);

                    //when ray hit a surface
                    if (intersecting)
                    {
                        float surfaceAngle = Vector3.Angle(hit.normal, -down);

                        teleportPoint = hit.point;
                        surfaceNormal = hit.normal;
                        teleportPossible = true;
                        ////Is surface flat enough to stand on?
                        //if (Mathf.Abs(surfaceAngle) <= maxSurfaceAngle)
                        //{

                        //}
                        break; //exit for loop since the target point was found
                    }
                    lastRaycastPoint = nextPoint;
                }//end of raycasting logic
            }//end of for loop


            //when all conditions for teleporting apply
            if (teleportPossible)
            {
                graphicsElements.arc.material = graphicsElements.activatedMaterial;
                graphicsElements.circle.material = graphicsElements.activatedMaterial;
            }
            else
            {
                graphicsElements.arc.material = graphicsElements.deactivatedMaterial;
                graphicsElements.circle.material = graphicsElements.deactivatedMaterial;
            }

            graphicsElements.circle.transform.position = teleportPoint - down.normalized * SC_VRRig.rigTransform.lossyScale.y * graphicsElements.circleOffset;
            graphicsElements.circle.transform.rotation = Quaternion.LookRotation(surfaceNormal) * Quaternion.Euler(90, 0, 0);
            graphicsElements.arc.transform.localScale = SC_VRRig.rigTransform.lossyScale;
            graphicsElements.circle.transform.localScale = SC_VRRig.rigTransform.lossyScale;

        }

        /// <summary>
        /// Get feet position of player. This differs from camerarig position, when the playercamera inside can move
        /// </summary>
        /// <returns>world space feet position of player</returns>
        private Vector3 GetGroundedPlayerPosition()
        {
            Vector3 playerGroundedPos = SC_VRRig.rigTransform.InverseTransformPoint(SC_VRRig.mainCamera.transform.position);
            playerGroundedPos.y = 0;
            playerGroundedPos = SC_VRRig.rigTransform.TransformPoint(playerGroundedPos);
            return playerGroundedPos;
        }

        /// <summary>
        /// create points for circle drawn at the end of the ray
        /// </summary>
        void CreateCircle()
        {
            Vector3[] points = new Vector3[graphicsElements.circleSegments];

            for (int ct = 0; ct < points.Length; ct++)
            {
                float rot = Mathf.Deg2Rad * ((float)ct / points.Length * 360f);
                points[ct] = new Vector3(Mathf.Sin(rot) * graphicsElements.circleRadius, 0, Mathf.Cos(rot) * graphicsElements.circleRadius);
            }

            graphicsElements.circle.positionCount = points.Length;
            graphicsElements.circle.SetPositions(points);
        }


        public static void Disable()
        {
            if (m_Main == null || m_Main.gameObject == null) return;
            m_Main.gameObject.SetActive(false);
        }

        public static void Enable()
        {
            if (m_Main == null || m_Main.gameObject == null) return;
            m_Main.gameObject.SetActive(true);
        }

    }
}
