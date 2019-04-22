using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;


public class RequestRoom
{
    public string roomcode;
};

public class JoinRoom
{
    public bool joined;
    public string username;
    public string failReason;
};
public class StartGame
{
    public string category;
    public string player1Name;
    public string player2Name;
};

public class EnterSubmission
{
    public int player;
    public string submission;
};

public class TimeChanged
{
    public int time;
};

public class Vote
{
    public float percentage;
};

public class Timeout
{
    public int winner;
};


public class socketHandler : MonoBehaviour
{
    public string serverURL = "ws://hackbox-backend.herokuapp.com/";

    protected Socket socket = null;
    private string roomcode;

    // Use this for initialization
    void Start()
    {
        roomcode = "-1";
        PrepareSocket();
    }

    /// <summary>
    /// Invoked when the game is over. We'll destroy the room when this happens
    /// </summary>
    void OnApplicationQuit()
    {
        if (socket != null) 
        {
            RequestRoom args = new RequestRoom();
            args.roomcode = roomcode;
            string strArgs = JsonUtility.ToJson(args);
            socket.Emit("close room", strArgs);
        }    
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
                //parse the message received from the socket
                RequestRoom room = JsonUtility.FromJson<RequestRoom>(data.ToString());
                roomcode = room.roomcode;

                //update the roomcode in the UI
                GameManager.DisplayRoomCode(roomcode);
            });

            //server indicated that a player joined the room. Store and display the new player in the player list
            socket.On("join room", (data) =>
            {
                JoinRoom join = JsonUtility.FromJson<JoinRoom>(data.ToString());

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

            //server has started an instance of the game. Store and display the category and players
            socket.On("start game", (data) =>
            {
                StartGame start = JsonUtility.FromJson<StartGame>(data.ToString());

                print("prompt: "+ start.category);
                print("player1: " + start.player1Name);
                print("player2: " + start.player2Name);

                //clear any previous answers + reset vote indicator
                GameManager.DisplayAnswer(0, "");
                GameManager.DisplayAnswer(1, "");
                GameManager.UpdateVote(0.5f);

                //update the game state to show the correct screen
                GameManager.StartGame(start.player1Name, start.player2Name, start.category);
            });

            //server has relayed a submission from a player. Display it on the screen
            socket.On("enter submission", (data) =>
            {
                EnterSubmission submission = JsonUtility.FromJson<EnterSubmission>(data.ToString());

                print("player " + submission.player + " submission: " + submission.submission);


                //update the game state to show the correct screen
                GameManager.DisplayAnswer(submission.player, submission.submission);
            });

            //server has caught another vote. update the percent shown on screen
            socket.On("vote", (data) =>
            {
                Vote vote = JsonUtility.FromJson<Vote>(data.ToString());

                //display the correct vote percent
                GameManager.UpdateVote(vote.percentage);
            });

            //server has sent timer updates. Display them
            socket.On("time changed", (data) =>
            {
                TimeChanged tc = JsonUtility.FromJson<TimeChanged>(data.ToString());

                //display the correct time
                GameManager.UpdateTime(tc.time);
            });

            //the server has indicated that the round has ended. Display the winner.
            socket.On("timeout", (data) =>
            {
                Timeout timeout = JsonUtility.FromJson<Timeout>(data.ToString());
                //display the correct time
                GameManager.EndGame(timeout.winner);
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
