using Sirenix.OdinInspector;
using UnityEngine;
using DependencyInjection;

[RequireComponent(typeof(EntityControls))]
public class EntityMove : MonoBehaviour
{
    [Inject] EntityControls Controls;
    Rigidbody rb;
    [SerializeField] float speedMultiplier; float origSpeed;
    [SerializeField] float maxVel;

    [Button]
    void UpdateOrigSpeed()
    {
        origSpeed = speedMultiplier;
    }

    private void Awake()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        if (Controls == null) Debug.LogError("No Controls found");
        UpdateOrigSpeed();
        rb.maxLinearVelocity = maxVel;
        rb.maxAngularVelocity = maxVel;
    }

    private void FixedUpdate()
    {
        MoveEntity();
    }

    void MoveEntity()
    {
        Vector2 moveInput = (Controls != null) ? Controls.move.Invoke() : Vector2.zero ;
        if (moveInput == Vector2.zero) return;

        if (Mathf.Abs(moveInput.x) > 0.5f && Mathf.Abs(moveInput.y) > 0.5f)
            speedMultiplier = origSpeed * 0.7f;
        else
            speedMultiplier = origSpeed;

       // print(moveInput + " mlt " + speedMultiplier);

        if (moveInput.x != 0)
        {
            if (moveInput.x > 0.5)
                rb.AddForce(Controls.bodyDirection.transform.right * speedMultiplier * 100);
            if (moveInput.x < 0.5)
                rb.AddForce(-Controls.bodyDirection.transform.right * speedMultiplier * 100);
        }
        if (moveInput.y != 0)
        {
            if (moveInput.y > 0.5)
                rb.AddForce(Controls.bodyDirection.transform.forward * speedMultiplier * 100);
            if (moveInput.y < 0.5)
                rb.AddForce(-Controls.bodyDirection.transform.forward * speedMultiplier * 100);
        }

        //print(rb.linearVelocity);

    }





}
