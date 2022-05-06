using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonWithSword // INHERITANCE
{
    [SerializeField] public PersonWithSword target;

    private float attackDistance = 1.0f;

    override protected void Update() // POLYMORPHISM
    {
        if (motionState == MotionState.Normal)
        {
            // Determine which direction to rotate towards
            Vector3 targetDirection = target.transform.position - transform.position;
            targetDirection.y = 0;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, speed * Time.deltaTime, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);

            if (targetDirection.magnitude < attackDistance)
            {
                StartCoroutine("Swing", 0.3f);
            }
            else if (target.motionState != MotionState.OutOfBounds && target.motionState != MotionState.Knockback)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        base.Update();
    }
}
