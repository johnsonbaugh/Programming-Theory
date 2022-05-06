using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : PersonWithSword // INHERITANCE
{
    override protected void Update() // POLYMORPHISM
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (motionState == MotionState.Normal || motionState == MotionState.SwordSwing)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
            transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * turnSpeed);

            if (Input.GetKeyDown(KeyCode.Space) && motionState == MotionState.Normal)
            {
                StartCoroutine("Spin", 1.5f);
            }
        }

        base.Update();
    }
}
