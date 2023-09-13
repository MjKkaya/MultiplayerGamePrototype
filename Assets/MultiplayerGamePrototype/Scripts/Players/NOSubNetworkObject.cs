using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;


public class NOSubNetworkObject : NetworkBehaviour
{
    void Start()
    {
        Debug.Log($"NOSubNetworkObject-Start-IsOwner:{IsOwner}");
        if (!IsOwner)
            return;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"NOSubNetworkObject-OnNetworkSpawn:{IsOwner}");
    }


    //[ServerRpc(RequireOwnership = false)]
    [ServerRpc()]
    public void TextServerRpc()
    {
        Debug.Log("NOSubNetworkObject-TextServerRpc");
    }


    private void Update()
    {
        if (IsOwner && Keyboard.current.kKey.wasPressedThisFrame) { 
            Debug.Log("NOSubNetworkObject-SendRPC");
            TextServerRpc();
        }
    }
}
