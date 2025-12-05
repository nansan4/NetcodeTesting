using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    InputAction moveAction;
    InputAction randNumAction;
    InputAction jumpAction;
    
    float moveSpeed = 3f;

    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // can change read/write permissions to allow client to change data
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56,
        _bool = true,
        
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    public override void OnNetworkSpawn()
    {
        // use OnNetworkSpawn instead of Start when messing with NetworkObjects
        //base.OnNetworkSpawn();
        moveAction = InputSystem.actions.FindAction("Move");
        randNumAction = InputSystem.actions.FindAction("Attack");
        jumpAction = InputSystem.actions.FindAction("Jump");

        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + ";" + newValue._bool + ";" + newValue.message);
        };
    }
    
    void Update()
    {
       
        if (!IsOwner) return;

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveValue.x, 0f, moveValue.y);
        moveDirection = transform.TransformDirection(moveDirection);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        if (jumpAction.WasPerformedThisFrame())
        {
            int randNumValue = Random.Range(0, 10);
            randomNumber.Value = new MyCustomData
            {
                _int = 10,
                _bool = false,
                message = "I can see you from across the world"
            };
        }
        //Vector3 moveDir = new Vector3(0, 0, 0);

        //if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        //if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        //if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        //if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        //float moveSpeed = 3f;
        //transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
