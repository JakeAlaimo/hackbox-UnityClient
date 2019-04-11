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
    public List<string> playerList = new List<string>();
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
    private string jsondata;



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
            print("1111");
            socket = IO.Socket(serverURL);
            socket.On(Socket.EVENT_CONNECT, () => {
                print("2222");
                if (roomcode=="-1"){
                    print("3333");
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
                    // socket.Emit("join room");
                    socket.On("join room", (data) =>
                    {
                    //Debug.Log(data.GetType());
                        print("444");
                        jsondata = data.ToString();

                        JoinResult join = JsonUtility.FromJson<JoinResult>(jsondata);
                        // print("in the update:" + room.roomcode);
                        if (join.joined == true){
                            print(join.username);
                        }else{
                            print("fail:"+join.username);
                        }
                        
                    //roomcode = JsonUtility.FromJson<string>(data);
                    //Dictionary<string, string> data = new Dictionary<string, string>();

                    });
                    socket.On("start game", (data) =>
                    {
                    //Debug.Log(data.GetType());
                        jsondata = data.ToString();

                        gameStatu game = JsonUtility.FromJson<gameStatu>(jsondata);
                        // print("in the update:" + room.roomcode);
                        Application.LoadLevel("GameScene");
                        // Application.loadedLevel("GameScene");
                        
                    //roomcode = JsonUtility.FromJson<string>(data);
                    //Dictionary<string, string> data = new Dictionary<string, string>();

                    });
                }
            });
             socket.On("join room", (data) =>
            {
                print("555");
                    //Debug.Log(data.GetType());
                jsondata = data.ToString();

                JoinResult join = JsonUtility.FromJson<JoinResult>(jsondata);
                        // print("in the update:" + room.roomcode);
                if (join.joined == true){
                    print(join.username);
                }else{
                    print("fail:"+join.username);
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
