using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Numena.SC
{
    /// <summary>
    /// Moves the VR rig in the direction of a controller using thumbstick input.
    /// </summary>
    public class FlyLocomotion : MonoBehaviour
    {
        bool secondaryLeft, secondaryRight;

        [SerializeField] float speed = 5;

        [Tooltip("Object that defines the forward and side vector for movement.")]
        [SerializeField] Transform directionAnchor;

        private void Update()
        {
            GetInput();
        }

        void GetInput()//todo: find a fix that allows using new input system
        {
            //Vector2 primary = primaryStick.action.ReadValue<Vector2>();
            //Vector2 secondary = secondaryStick.action.ReadValue<Vector2>();
            //Note: there is a bug in Unitys InputSystem that causes the left controller to not work correctly. this is a workaround.
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 primary);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 secondary);

            Move(primary);

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

        void Move(Vector2 dir)
        {
            SC_VRRig.rigTransform.Translate((directionAnchor.forward * dir.y + directionAnchor.right * dir.x) * speed * Time.deltaTime, Space.World);
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
    }
}
