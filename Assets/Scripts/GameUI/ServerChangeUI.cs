using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class ServerChangeUI : MonoBehaviour
{
    [SerializeField] private Dropdown ipDropdown;

    void Start()
    {
        ipDropdown.ClearOptions();
        ipDropdown.AddOptions(NetworkManager.Instance.GetAvaliableServerIps());
        ipDropdown.value = NetworkManager.Instance.GetAvaliableServerIps().FindIndex(ip => ip == NetworkManager.Instance.GetServerIp());
    }

    public void OnChange() {
        NetworkManager.Instance.SetServerIp(NetworkManager.Instance.GetAvaliableServerIps()[ipDropdown.value]);
        NetworkManager.Instance.ReConnect();
    }
}
