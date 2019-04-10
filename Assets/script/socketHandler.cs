using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

public class RoomData
{
    public string roomcode;
    public string onlineNumber;
    public List<string> playerList = new List<string>();
};

public class socketHandler : MonoBehaviour
{
    public string serverURL = "https://hackbox-backend.herokuapp.com/";

    //public InputField uiInput = null;
    //public Button uiSend = null;
    //public Text uiChatLog = null;

    protected Socket socket = null;
    //protected List<string> chatLog = new List<string>();
    private string roomcode;
    public Text code;



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
        DoOpen();


    }
    void Update()
    {
        if (roomcode != "-1")
        {
            code.text = roomcode;
        }
        
    }

    void DoOpen()
    {
       
        if (socket == null)
        {
            socket = IO.Socket(serverURL);
            socket.On(Socket.EVENT_CONNECT, () => {
                if (roomcode=="-1"){
                    socket.Emit("request room");
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
                }
            });

        }

    }

    void startGame(string roomcode)
    {
        if (socket != null)
        {
           
            socket.On(Socket.EVENT_CONNECT, () => {
                socket.Emit("start game");
                socket.On("start game", (data) =>
                {
                    //Debug.Log(data.GetType());

                    roomcode = data.ToString();

                    if (roomcode != "-1")
                    {


                    }
                    //roomcode = JsonUtility.FromJson<string>(data);
                    //Dictionary<string, string> data = new Dictionary<string, string>();

                });

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
