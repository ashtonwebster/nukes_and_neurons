using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AIMouseLook : MouseLook {

	protected override void GetRotationInput ()
	{
		this.xInput = 0;
		this.yInput = 3;
	}

}
