using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Turning : State
    {
        public Turning(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Turning;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);
            
            MovementSystem.SetTargetSpeed(MovementSystem.turningSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // if (MovementSystem.cameraLook.mouseX > 1f) return;
            //
            // if (MovementSystem.inputDetection.movementInput.z > 0) MovementSystem.ChangeState(MovementSystem._walkingState);
            // else if (MovementSystem.inputDetection.movementInput.z < 0) MovementSystem.ChangeState(MovementSystem._backRunningState);
            // else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
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
                    movementCurve = MovementSystem.moveToTurn;
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.fastToSlow;
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.moveToTurn;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.moveToTurn;
                    break;
                case StateName.InAir:
                    movementCurve = MovementSystem.moveToTurn;
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.Turning:
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    movementCurve = MovementSystem.moveToTurn;
                    break;
            }
        }
    }
}

