using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonWithSword : MonoBehaviour
{
    [SerializeField] protected GameObject swordArm;
    [SerializeField] protected GameObject sword;
    [SerializeField] protected Collider groundCollider;
    [SerializeField] protected float speed = 10.0f;

    protected float turnSpeed = 120.0f;
    protected float hitForce = 10.0f;

    private Rigidbody personRb;
    private float lowerBound = -10.0f;
    private Vector3 startPos;

    public MotionState motionState { get; private set; }

    public enum MotionState
    {
        Normal,
        SwordSwing,
        SwordSpin,
        Knockback,
        OutOfBounds
    }

    protected virtual void Start()
    {
        sword.GetComponent<Collider>().enabled = false;
        personRb = GetComponent<Rigidbody>();
        startPos = transform.position;
        motionState = MotionState.Normal;
    }

    private void Reset()
    {
        transform.position = startPos;
        personRb.velocity = Vector3.zero;
        motionState = MotionState.Normal;
    }

    // Called after child update
    protected virtual void Update()
    {
        if (transform.position.y < -0.5f)
            motionState = MotionState.OutOfBounds;

        if (transform.position.y < lowerBound)
        {
            Reset();
        }
    }

    IEnumerator Swing(float duration)
    {
        sword.GetComponent<Collider>().enabled = true;
        motionState = MotionState.SwordSwing;

        yield return SwingMotion(duration, 90);
        yield return SwingMotion(duration, -90);

        sword.GetComponent<Collider>().enabled = false;
        motionState = MotionState.Normal;
    }

    IEnumerator Spin(float duration)
    {
        sword.GetComponent<Collider>().enabled = true;
        motionState = MotionState.SwordSpin;

        yield return SwingMotion(0.1f, 90);

        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f * 3;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }

        yield return SwingMotion(0.1f, -90);

        sword.GetComponent<Collider>().enabled = false;
        motionState = MotionState.Normal;
    }

    IEnumerator SwingMotion(float duration, int degrees)
    {
        Transform swordTransform = swordArm.transform;
        float startRotation = swordTransform.eulerAngles.x;
        float endRotation = startRotation - degrees;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float xRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            swordTransform.eulerAngles = new Vector3(xRotation, swordTransform.eulerAngles.y, swordTransform.eulerAngles.z);
            yield return null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " hit " + motionState + " " + other.gameObject.name);
        if (other.CompareTag("Hittable"))
        {
            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            PersonWithSword enemy = other.gameObject.GetComponent<PersonWithSword>();

            if (enemy.motionState != MotionState.Knockback)
            {
                enemy.motionState = MotionState.Knockback;
                enemy.groundCollider.isTrigger = true;
                enemyRb.velocity = Vector3.zero;
                enemyRb.AddForce((other.gameObject.transform.position - transform.position).normalized * hitForce + Vector3.up * 5.0f, ForceMode.Impulse);
            }
        }
        else if (other.CompareTag("Ground"))
        {
            motionState = MotionState.Normal;
            groundCollider.isTrigger = false;
        }
    }
}