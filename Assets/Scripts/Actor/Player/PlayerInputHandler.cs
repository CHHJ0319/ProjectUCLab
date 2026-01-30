using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        private bool _jumpTriggered;
        public bool JumpTriggered
        {
            get
            {
                if (_jumpTriggered)
                {
                    _jumpTriggered = false;
                    return true;
                }
                return false;
            }
        }

        public void OnMove(InputValue value)
        {
            Vector2 movement = value.Get<Vector2>();
            Horizontal = movement.x;
            Vertical = movement.y;
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpTriggered = true;
            }
        }
    }
}
