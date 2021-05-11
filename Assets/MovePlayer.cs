using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    float speed = 3f;
    [SerializeField]
    float rotSpeed = 3f;
    Transform cameraTransform;
    Rigidbody rigidbody;
    Vector3 moveDir;
    Vector3 initialPos;
    public float minPossibleY = -50f; // prag sub care nu mai exista platforme

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveDirection();

        ApplyRootMotion();

        ApplyRootRotation();

        HandleJump();
    }

    private void HandleJump()
    {
        if (transform.position.y < minPossibleY)
            transform.position = initialPos; // a cazut de pe platforma, il teleportam la pozitia initiala;
    }

    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 10e-3f) // rotim doar daca directia de miscare este nenula
            return;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        // roteste smooth de la rotatia curenta la rotatia tinta
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        
    }

    private void ApplyRootMotion()
    {
        // transform.position += dir * Time.deltaTime; doar daca nu avem rigidbody atasat
        float velY = rigidbody.velocity.y; // pastram viteza pe axa verticala, calculata de motorul fizic, pentru gravitatie
        rigidbody.velocity = moveDir * speed; // suprascriem viteza corpului cu directia de miscare
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, velY, rigidbody.velocity.z); // pentru gravitatie, cadere libera
    }

    private void GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDir = cameraTransform.right * h + cameraTransform.forward * v; // directia de miscare relativa la spatiul camerei
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; //directia de miscare este in plan orizontal( fara sus jos) directiile sunt vectori normalizati(lungime 1)
    }

}
