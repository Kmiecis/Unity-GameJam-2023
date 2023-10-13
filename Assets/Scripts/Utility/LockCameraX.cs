using Cinemachine;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class LockCameraX : CinemachineExtension
    {
        [Tooltip("Lock the camera's Z position to this value")]
        public float _xPosition;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (enabled && stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos.x = _xPosition;
                state.RawPosition = pos;
            }
        }
    }
}