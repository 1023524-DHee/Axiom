using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Walking : State
    {
        private float _toRunCounter;
        
        public Walking(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Walking;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);
            
            MovementSystem.SetDrag(MovementSystem.groundedDrag);
            MovementSystem.SetGravity(MovementSystem.groundGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(Mathf.Abs(MovementSystem._rb.velocity.magnitude - MovementSystem.currentTargetSpeed) < 1f) _toRunCounter += Time.deltaTime;
            else _toRunCounter = 0f;
            
            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z < 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem.inputDetection.movementInput.z > 0)
            {
                if(_toRunCounter > 1f) MovementSystem.ChangeState(MovementSystem._runningState);
                else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            }
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        protected override void SelectMovementCurve()
        {
            base.SelectMovementCurve();
            
            switch (previousState)
            {
                case StateName.Idle:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Walking:
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.InAir:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
            }
        }
    }
}