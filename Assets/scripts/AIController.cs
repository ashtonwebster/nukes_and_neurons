﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AIController : FirstPersonController {

	//this may ignore the public settings...
	protected AIMouseLook _m_AIMouseLook = new AIMouseLook();

	protected override MouseLook m_MouseLook {
		get {
			return _m_AIMouseLook;
		}
	}

	protected override void GetInput(out float speed) {
		// Read input
		//manually setting
		//float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float horizontal = 20f;
		//float vertical = CrossPlatformInputManager.GetAxis("Vertical");
		float vertical = 0f;

		bool waswalking = this.m_IsWalking;

		#if !MOBILE_INPUT
		// On standalone builds, walk/run speed is modified by a key press.
		// keep track of whether or not the character is walking or running
		m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
		#endif
		// set the desired speed to be walking or running
		speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
		m_Input = new Vector2(horizontal, vertical);

		// normalize input if it exceeds 1 in combined length:
		if (m_Input.sqrMagnitude > 1)
		{
			m_Input.Normalize();
		}

		// handle speed change to give an fov kick
		// only if the player is going to a run, is running and the fovkick is to be used
		if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
		{
			StopAllCoroutines();
			StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
		}
	}
}