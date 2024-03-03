using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualDiveInput(bool virtualDiveState)
        {
            starterAssetsInputs.DiveInput(virtualDiveState);
        }

        public void VirtualDashInput(bool virtualDashState)
        {
            starterAssetsInputs.DashInput(virtualDashState);
        }

        public void VirtualAttackInput(bool virtualAttackState)
        {
            starterAssetsInputs.AttackInput(virtualAttackState);
        }
    }

}
