using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;


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
public class GameStatus
{
    public string category;
    public string player1Name;
    public string player2Name;
    public string Timecount;
};


public class socketHandler : MonoBehaviour
{
    public string serverURL = "ws://hackbox-backend.herokuapp.com/";

    protected Socket socket = null;
    private string roomcode;

    public Text code;
    private string jsondata;

    GameObject waitingPanel;
    GameObject gameRoom;

    GameStatus stat;

    Text onlinePlayer;
    Text timer;


    // Use this for initialization
    void Start()
    {
        roomcode = "-1";

        stat = new GameStatus();
        stat.Timecount = "-1";
        onlinePlayer = GameObject.Find("onlinePlayer").GetComponent<Text>();
        timer = GameObject.Find("Timer").GetComponent<Text>();
        print("player: "+onlinePlayer.text);

        PrepareSocket();
    }

    void Update()
    {
        //if (playerCount < playerList.Count)
        //{
        //    //print("1111");
        //    //Text onlinePlayer = GameObject.Find("onlinePlayer").GetComponent<Text>();
        //    onlinePlayer.text = onlinePlayer.text +"  "+ join.username;
        //    playerCount = playerList.Count;
        //}
        //if (stat.Timecount != "-1")
        //{
        //    //print("22222");
        //    timer.text = "Timer: " + stat.Timecount;
        //}
       
        //print(timer.text);



    }

    void PrepareSocket()
    {
        if (socket == null)
        {
            print("Initializing socket");
            socket = IO.Socket(serverURL);

            //on connection to the server, request a room
            socket.Once(Socket.EVENT_CONNECT, () => {
                print("Socket connected");

                if(roomcode == "-1") //only request a room if this is not some kind of reconnection
                    socket.Emit("request room");                
            });

            //server created the room and sent the code back. Store and display the code
            socket.On("request room", (data) =>
            {
                jsondata = data.ToString();

                //parse the message received from the socket
                RoomData room = JsonUtility.FromJson<RoomData>(jsondata);
                roomcode = room.roomcode;

                //update the roomcode in the UI
                GameManager.DisplayRoomCode(roomcode);
            });

            //server indicated that a player joined the room. Store and display the new player in the player list
            socket.On("join room", (data) =>
            {
                jsondata = data.ToString();

                JoinResult join = JsonUtility.FromJson<JoinResult>(jsondata);

                if (join.joined == true) //add the user to the player list
                {
                    print("Joined: " + join.username);
                    GameManager.AddPlayer(join.username);
                }
                else //inidicate that the given user failed to enter the room (probably due to a duplicate name)
                {
                    print("fail:"+join.username);
                }
            });

            socket.On("start game", (data) =>
            {
                //Debug.Log(data.GetType()); 
                print("game start");
                jsondata = data.ToString();
                print("before parse json");
                stat = JsonUtility.FromJson<GameStatus>(jsondata);
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
            });

            socket.On("time changed", (data) =>
            {
                //Debug.Log(data.GetType());
                print("In time changed event");
                jsondata = data.ToString();

                stat = JsonUtility.FromJson<GameStatus>(jsondata);
                // print("in the update:" + room.roomcode);
                if (stat.Timecount != "-1")
                {

                    print(stat.Timecount);
                }
                else
                {
                    print("fail:" + stat.Timecount);
                }

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

}
