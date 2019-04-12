using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
// using UnityEngine.SceneManagement;

public class RoomData
{
    public string roomcode;

    
};
public class JoinResult
{
    public bool joined;
    public string username;
    public string failReason;
};
public class gameStatu
{
    public string category;
    public string player1Name;
    public string player2Name;
    public string Timecount;
};


public class socketHandler : MonoBehaviour
{
    public string serverURL = "ws://hackbox-backend.herokuapp.com/";

    //public InputField uiInput = null;
    //public Button uiSend = null;
    //public Text uiChatLog = null;

    protected Socket socket = null;
    //protected List<string> chatLog = new List<string>();
    private string roomcode;
    public Text code;
    private string jsondata;
    GameObject waitingPanel;
    GameObject gameRoom;
    gameStatu stat;
    int playerCount;
    JoinResult join;
    Text onlinePlayer;
    Text timer;
    List<string> playerList = new List<string>();

    void Awake()
    {

    }
    void Destroy()
    {
        // DoClose();
    }

    // Use this for initialization
    void Start()
    {
        roomcode = "-1";
        playerCount = 0;
        //print("playerCount:" + playerList.Count);

        stat = new gameStatu();
        stat.Timecount = "-1";
        onlinePlayer = GameObject.Find("onlinePlayer").GetComponent<Text>();
        timer = GameObject.Find("Timer").GetComponent<Text>();
        print("player: "+onlinePlayer.text);

        //waitingPanel = GameObject.Find("waitingROOM");
        //waitingPanel.SetActive(false);
        DoOpen();


    }
    void Update()
    {
        if (roomcode != "-1")
        {
            code.text = roomcode;
        }

        //print("playercount:" + playerCount);
        //print("playerLisr.Count:" + playerList.Count);
        if (playerCount < playerList.Count)
        {
            //print("1111");
            //Text onlinePlayer = GameObject.Find("onlinePlayer").GetComponent<Text>();
            onlinePlayer.text = onlinePlayer.text +"  "+ join.username;
            playerCount = playerList.Count;
        }
        //if (stat.Timecount != "-1")
        //{
        //    //print("22222");
        //    timer.text = "Timer: " + stat.Timecount;
        //}
       
        //print(timer.text);



    }

    void DoOpen()
    {
        if (socket == null)
        {
            print("Initializing socket");
            socket = IO.Socket(serverURL);
            socket.Once(Socket.EVENT_CONNECT, () => {
                print("Socket connected");
                print(roomcode);
                if (roomcode == "-1")
                {
                    socket.Emit("request room");
                }
            });
            socket.On("request room", (data) =>
            {
            //Debug.Log(data.GetType());
                roomcode = data.ToString();

                RoomData room = JsonUtility.FromJson<RoomData>(roomcode);
                // print("in the update:" + room.roomcode);
                roomcode = room.roomcode;
            //roomcode = JsonUtility.FromJson<string>(data);
            //Dictionary<string, string> data = new Dictionary<string, string>();

            });
            // socket.Emit("join room");
            socket.On("join room", (data) =>
            {
            //Debug.Log(data.GetType());
                //print("444");
                jsondata = data.ToString();

                join = JsonUtility.FromJson<JoinResult>(jsondata);
                // print("in the update:" + room.roomcode);
                if (join.joined == true){
                    print(join.username);
                    playerList.Add(join.username);
                    //print("playerCount:" + playerList.Count);
                }
                else{
                    print("fail:"+join.username);
                }
                
            //roomcode = JsonUtility.FromJson<string>(data);
            //Dictionary<string, string> data = new Dictionary<string, string>();

            });
            socket.On("start game", (data) =>
            {
                //Debug.Log(data.GetType()); 
                print("game start");
                jsondata = data.ToString();
                print("before parse json");
                stat = JsonUtility.FromJson<gameStatu>(jsondata);
                // print("in the update:" + room.roomcode);
                //Application.LoadLevel("GameScene");
                // Application.loadedLevel("GameScene");
                print("stat:"+ stat.category);
                print("player1_before:" + stat.player1Name);
                print("player2_before:" + stat.player2Name);

                waitingPanel = GameObject.Find("waitingROOM");
                waitingPanel.SetActive(false);
                print("player1:"+stat.player1Name);
                print("player2:" + stat.player2Name);
                //roomcode = JsonUtility.FromJson<string>(data);
                //Dictionary<string, string> data = new Dictionary<string, string>();
            });
            socket.On("time changed", (data) =>
            {
                //Debug.Log(data.GetType());
                print("In time changed event");
                jsondata = data.ToString();

                stat = JsonUtility.FromJson<gameStatu>(jsondata);
                // print("in the update:" + room.roomcode);
                if (stat.Timecount != "-1")
                {

                    print(stat.Timecount);
                }
                else
                {
                    print("fail:" + stat.Timecount);
                }

                //roomcode = JsonUtility.FromJson<string>(data);
                //Dictionary<string, string> data = new Dictionary<string, string>();

            });
            socket.On("game_error", (data) =>
            {
                print(data);
            });

            socket.On(Socket.EVENT_DISCONNECT, () => {
                print("there was a disconnect");
            });
            socket.On(Socket.EVENT_RECONNECT, () => {
                print("Reconnected");
            });
        }

    }



    //void DoClose()
    //{
    //    if (socket != null)
    //    {
    //        socket.Disconnect();
    //        socket = null;
    //    }
    //}

    //void SendChat(string str)
    //{
    //    if (socket != null)
    //    {
    //        socket.Emit("chat", str);
    //    }
    //}
}
