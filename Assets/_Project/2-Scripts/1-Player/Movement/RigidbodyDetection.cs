using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RigidbodyDetection : MonoBehaviour
    {
        public Transform orientation;

        [Header("Ground Detection")]
        public float groundDetectorRadius = 0.5f;
        public Transform groundDetector;
        public LayerMask groundLayer;

        [Header("WallRun Detection")]
        public Transform wallDetector;
        public float wallCheckDistance = 0.5f;
        public LayerMask wallLayer;

        [Header("WallClimb Detection")] 
        public float sphereCastRadius;
        public float wallClimbDetectionLength;

        public RaycastHit slopeHit;
        private RaycastHit rightWallHit;
        private RaycastHit rightFrontWallHit;
        private RaycastHit rightBackWallHit;
        private RaycastHit leftWallHit;
        private RaycastHit leftFrontWallHit;
        private RaycastHit leftBackWallHit;

        public bool isGrounded { get; private set; }
        public bool isOnSlope { get; private set; }
        
        #region WallRun Detection
        public bool leftWallDetected {get; private set; }
        public bool leftFrontWallDetected {get; private set; }
        public bool leftBackWallDetected {get; private set; }
        public bool rightWallDetected { get; private set; }
        public bool rightFrontWallDetected { get; private set; }
        public bool rightBackWallDetected { get; private set; }
        #endregion
        
        #region WallClimb Detection
        public bool wallFrontDetected { get; private set; }
        public float wallLookAngle { get; private set; }
        
        public bool canWallClimb { get; private set; }
        #endregion
        
        public event Action OnPlayerLanded;
        public event Action OnSlopeEnded;

        private void Update()
        {
            GroundDetection();
            SlopeDetection();
            WallRunDetection();
            WallClimbCheck();
            
            Debug.Log(canWallClimb);
        }

        #region Ground Functions
        private void GroundDetection()
        {
            bool previouslyGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundDetector.position, groundDetectorRadius, groundLayer);
            if (!previouslyGrounded && isGrounded) OnPlayerLanded?.Invoke();
        }
        #endregion
        
        #region Slope Functions
        private void SlopeDetection()
        {
            bool wasOnSlope = isOnSlope;

            if (Physics.Raycast(groundDetector.position, groundDetector.TransformDirection(Vector3.down), out slopeHit,
                    groundDetectorRadius, groundLayer))
            {
                isOnSlope = slopeHit.normal != Vector3.up;
            }
            else isOnSlope = false;

            if (wasOnSlope && !isOnSlope) OnSlopeEnded?.Invoke();
        }
        #endregion
        
        #region WallRun Functions
        private void WallRunDetection()
        {
            var position = wallDetector.position;
            var right = orientation.right;
            var back = -orientation.forward;
            
            rightWallDetected = Physics.Raycast(position, right.normalized, out rightWallHit, wallCheckDistance, wallLayer);
            leftWallDetected = Physics.Raycast(position, -right.normalized, out leftWallHit, wallCheckDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (right + -back).normalized, out rightFrontWallHit, wallCheckDistance * 1.2f, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (-right + -back).normalized, out leftFrontWallHit, wallCheckDistance * 1.2f, wallLayer);           
            rightBackWallDetected = Physics.Raycast(position, (right + back).normalized, out rightBackWallHit, wallCheckDistance * 1.2f, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-right + back).normalized, out leftBackWallHit, wallCheckDistance * 1.2f, wallLayer);
        }

        public bool IsLeftWallDetected() => leftWallDetected || leftFrontWallDetected || leftBackWallDetected;
        public bool IsRightWallDetected() => rightWallDetected || rightFrontWallDetected || rightBackWallDetected;
        
        public Vector3 GetLeftWallNormal()
        {
            if (leftWallDetected) return leftWallHit.normal;
            if (leftFrontWallDetected) return leftFrontWallHit.normal;
            if (leftBackWallDetected) return leftBackWallHit.normal;
            return Vector3.zero;
        }

        public Vector3 GetRightWallNormal()
        {
            if (rightWallDetected) return rightWallHit.normal;
            if (rightFrontWallDetected) return rightFrontWallHit.normal;
            if (rightBackWallDetected) return rightBackWallHit.normal;
            return Vector3.zero;
        }

        public Transform GetLeftWall()
        {
            if (leftWallDetected) return leftWallHit.transform;
            if (leftFrontWallDetected) return leftFrontWallHit.transform;
            if (leftBackWallDetected) return leftBackWallHit.transform;
            return null;
        }

        public Transform GetRightWall()
        {
            if (rightWallDetected) return rightWallHit.transform;
            if (rightFrontWallDetected) return rightFrontWallHit.transform;
            if (rightBackWallDetected) return rightBackWallHit.transform;
            return null;
        }
        #endregion
        
        private void WallClimbCheck()
        {
            var position = wallDetector.position;
            var forward = orientation.forward;
            var up = transform.up;

            bool rightDetected = Physics.Raycast(position, (Quaternion.AngleAxis(15f, up) * forward).normalized, out _, wallClimbDetectionLength, wallLayer);
            bool leftDetected = Physics.Raycast(position, (Quaternion.AngleAxis(-15f, up) * forward).normalized, out _, wallClimbDetectionLength, wallLayer);   

            if (rightDetected && leftDetected) canWallClimb = true;
            else canWallClimb = false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(wallDetector.position, wallDetector.position + (Quaternion.AngleAxis(15f, wallDetector.up) * orientation.forward).normalized * wallClimbDetectionLength);
            Gizmos.DrawLine(wallDetector.position, wallDetector.position + (Quaternion.AngleAxis(-15f, wallDetector.up) * orientation.forward).normalized * wallClimbDetectionLength);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + orientation.right.normalized * wallCheckDistance);
            Gizmos.DrawLine(wallDetector.position, orientation.position + -orientation.right.normalized * wallCheckDistance);
            Gizmos.DrawLine(wallDetector.position, orientation.position + (orientation.right + orientation.forward).normalized * wallCheckDistance * 1.2f);
            Gizmos.DrawLine(wallDetector.position, orientation.position + (-orientation.right + orientation.forward).normalized * wallCheckDistance * 1.2f);
            Gizmos.DrawLine(wallDetector.position, orientation.position + (orientation.right + -orientation.forward).normalized * wallCheckDistance * 1.2f);
            Gizmos.DrawLine(wallDetector.position, orientation.position + (-orientation.right + -orientation.forward).normalized * wallCheckDistance * 1.2f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);
        }
    }
}

