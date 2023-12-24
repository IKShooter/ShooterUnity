using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private static BattleUI instance;
    public static BattleUI Instance => instance;

    public Text sceneName;
    public Text roomName;

    public GameObject contentScrollView;
    public GameObject panelPlayerInTab;
    public GameObject panelReservPlayerInTab;

    public Room Room = null;
    public List<Player> Players => Room.Players;

    public Player[] TestPlayers;

    private void Awake()
    {
        panelPlayerInTab = Resources.Load<GameObject>("Prefabs/PanelPlayerInTab");
    }

    void Start()
    {

    }

    void Update()
    {
        Room = ClientListener.CurrenRoom;

        if (instance == null) instance = this;

        if(Room != null)
        {
            sceneName.text = Room.SceneName;
            roomName.text = Room.NameRoom;
        }

        if(contentScrollView.transform.childCount == 0)
        {
            SpawnPlayersInTabList();
        }
    }

    public void DeleteAllContentScrollViewInTab()
    {
        for (int i = 0; i < contentScrollView.transform.childCount; i++)
        {
            Destroy(contentScrollView.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnPlayersInTabList()
    {
        DeleteAllContentScrollViewInTab();

        foreach(Player player in ClientListener.CurrenRoom.Players)
        {
            panelReservPlayerInTab = Instantiate(panelPlayerInTab, contentScrollView.transform);

            panelReservPlayerInTab.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
            panelReservPlayerInTab.transform.GetChild(1).GetComponent<Text>().text = player.Id.ToString();
            panelReservPlayerInTab.transform.GetChild(2).GetComponent<Text>().text = "0";
        }
    }
}
