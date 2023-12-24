using LiteNetLib.Utils;
using LiteNetLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class AuthMenu : MonoBehaviour
{
    GameObject GClientConnect;

    public InputField nickname;

    static NetManager Client => ClientConnect.client;

    private void Awake()
    {
        GClientConnect = new GameObject();
        GClientConnect.name = "ClientConnect";
        GClientConnect.AddComponent<ClientConnect>();
        DontDestroyOnLoad(GClientConnect);
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    public string GetUniqueIdentifier()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public void Auth()
    {
        Auth auth = new Auth()
        {
            codeAuth = GetUniqueIdentifier(),
            nickname = nickname.text
        };

        string data = JsonConvert.SerializeObject(auth);

        ActionMessage actionMessage = new ActionMessage()
        {
            ActionCode = ActionCode.Auth,
            Data = data
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)actionMessage.ActionCode);
        writer.Put(actionMessage.Data);
        Client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public struct ActionMessage
    {
        public ActionCode ActionCode { get; set; }
        public string Data { get; set; }
    }
}