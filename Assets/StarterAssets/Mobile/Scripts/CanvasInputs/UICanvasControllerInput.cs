using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        private bool _isInputAtive = true;

        public void SetActiveInputs(bool isActive)
        {
            _isInputAtive = isActive;
            // starterAssetsInputs.enabled = isActive;
        }

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            if(_isInputAtive)
                starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            if(_isInputAtive)
                starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            if(_isInputAtive)
                starterAssetsInputs.JumpInput(virtualJumpState);
        }

        // public void VirtualSprintInput(bool virtualSprintState)
        // {
            // if(_isInputAtive)
        //     starterAssetsInputs.SprintInput(virtualSprintState);
        // }

        public void VirtualShotInput(bool virtualSprintState)
        {
            if(_isInputAtive)
                starterAssetsInputs.ShotInput(virtualSprintState);
        }

        public void VirtualBombInput(bool virtualSprintState)
        {
            if(_isInputAtive)
                starterAssetsInputs.BombInput(virtualSprintState);
        }
    }
}