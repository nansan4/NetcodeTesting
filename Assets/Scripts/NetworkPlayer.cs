using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    InputAction moveAction;
    InputAction randNumAction;
    
    float moveSpeed = 3f;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        randNumAction = InputSystem.actions.FindAction("Attack");
        
    }
    void Update()
    {
        Debug.Log(OwnerClientId + "; " + "randnum" + randomNumber.Value);
        if (!IsOwner) return;

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveValue.x, 0f, moveValue.y);
        moveDirection = transform.TransformDirection(moveDirection);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        int randNumValue = randomNumber.Value;
        randomNumber.Value = randNumValue;
        //Vector3 moveDir = new Vector3(0, 0, 0);

        //if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        //if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        //if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        //if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        //float moveSpeed = 3f;
        //transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
