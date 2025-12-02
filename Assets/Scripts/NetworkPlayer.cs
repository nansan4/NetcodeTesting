using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    InputAction moveAction;
    
    float moveSpeed = 3f;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        
    }
    void Update()
    {
        if (!IsOwner) return;

        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        //Vector3 moveDir = new Vector3(0, 0, 0);

        //if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        //if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        //if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        //if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        //float moveSpeed = 3f;
        //transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
