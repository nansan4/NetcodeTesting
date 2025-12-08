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

    [SerializeField] private Transform spawnedObjectPrefab;

    private Transform spawnedObjectTransform;

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
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true); 
            int randNumValue = Random.Range(0, 10);
            randomNumber.Value = new MyCustomData
            {
                _int = 10,
                _bool = false,
                message = "I can see you from across the world"
            };
           
        }
        if (randNumAction.WasPerformedThisFrame())
        {
            Destroy(spawnedObjectTransform.gameObject);
        }
        
    }
    [ServerRpc]
    private void TestServerRpc(string message, ServerRpcParams serverRpcParams) // need to end with ServerRpc in name and be defined inside a NetworkBehaviour
    {
        Debug.Log("ServerRpc" + OwnerClientId + ";" + message + ";" + serverRpcParams.Receive.SenderClientId);
        // using ServerRpc does not run the code on the client, only on the server
    }
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("ServerRpc" + OwnerClientId);
        // ClientRpc is called on the server, but then run on the client.
        // Client cannot call ClientRpc so you can only use on the server
        // has the same things as ServerRpc besides params
        // TestClientRpc(new ClientRpcParams { Send = new ClientRpcParams { TargetClientIds = new List<uint> { 1 } });
        // sends to specific client
    }
}
