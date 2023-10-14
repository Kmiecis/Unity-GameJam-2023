using Cinemachine;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class MinCameraY : CinemachineExtension
    {
        [Tooltip("Lock the camera's Y position to this value")]
        public float _yPosition;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (enabled && stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                if (pos.y < _yPosition)
                {
                    pos.y = _yPosition;
                    state.RawPosition = pos;
                }
            }
        }
    }
}