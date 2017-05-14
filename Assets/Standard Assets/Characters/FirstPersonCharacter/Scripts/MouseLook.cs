using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
		public float joyXSensitivity = 50f;
		public float joyYSensitivity = 50f;
        public bool clampVerticalRotation = false;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;
		// exposed to AIMouseLook
		protected float yInput;
		protected float xInput;

		protected float yInput1;
		protected float xInput1;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

		/// <summary>
		/// sets the rotation x and y values.  This allows the AIMouseLook to overwrite
		/// the values.  This might be the opposite of what you expect for x and y,
		/// but you are rotating AROUND these AXES, not the direction of the movement.
		/// </summary>
		protected virtual void GetRotationInput(bool usingJoystick) {
			if (!usingJoystick) {
				this.yInput = CrossPlatformInputManager.GetAxis ("Mouse X") * XSensitivity;
				this.xInput = CrossPlatformInputManager.GetAxis ("Mouse Y") * YSensitivity;
			} else {
				this.yInput = CrossPlatformInputManager.GetAxis ("JoyAltX") * joyXSensitivity;
				this.xInput = CrossPlatformInputManager.GetAxis ("JoyAltY") * joyYSensitivity;
			}
		}

		public void LookRotation(Transform character, Transform camera) {
			LookRotation (character, camera, false);
		}

		public void LookRotation(Transform character, Transform camera, bool usingJoystick)
        {
			this.GetRotationInput (usingJoystick);
			float xRot = this.xInput * XSensitivity;
			float yRot = this.yInput * YSensitivity;
            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
			this.yInput = 0;
			this.xInput = 0;
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
