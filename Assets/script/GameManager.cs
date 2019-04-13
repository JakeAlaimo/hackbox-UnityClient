using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void Command();

public class GameManager : MonoBehaviour {

    private static Queue<Command> commands; //all of the operations the game manager should perform, in order

    //game-state information:
    static string roomCode = "";
    static List<string> playerNames;
    static float gameTime = 0;

    static string competingPlayer1 = "";
    static string competingPlayer2 = "";
    static string prompt = "";

    static float votePercent = 0.5f;


    // Use this for initialization
    void Start () {

        playerNames = new List<string>();
        commands = new Queue<Command>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        while (commands.Count > 0)
        {
            //we need to lock prior to executing commands. The socket event listener threads might try to modify the queue at the same time we are executing
            lock (commands)
            {
                commands.Dequeue().Invoke(); //run the the next command
            } //unlock following the execution of a single command. Allows new commands to be added to the queue as quickly as possible
        }
	}

    /// <summary>
    /// Tells the GameManager to perform a new game-related task.
    /// </summary>
    /// <param name="cmd">The callback function the GameManager should perform.</param>
    private static void AddCommand(Command cmd)
    {
        //we can only add to the queue while nothing else is accessing it
        lock (commands)
        {
            commands.Enqueue(cmd); //add the command to the queue and immediately release the lock
        }
    }


    //////////////////////////////////////////////////////////////////// public static interface functions ////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void DisplayRoomCode(string rmCd)
    {
        lock (roomCode) //probably won't be a factor as roomCode is only set once
        {
            roomCode = rmCd;
        }

        //now tell the main thread to display the roomcode on the UI
        AddCommand(() => { GameObject.Find("code").GetComponent<Text>().text = roomCode; });
    }

    public static void AddPlayer(string playerName)
    {
        lock (playerNames) //add player only when other threads aren't reading from the list or modifying it
        {
            playerNames.Add(playerName);
        }

        //now tell the main thread to add the player's name to the player display
        AddCommand(() => { GameObject.Find("onlinePlayers").GetComponent<Text>().text += " " + playerName; });
    }

    public static void StartGame(string p1, string p2, string category)
    {
        lock (competingPlayer1) //once again, start room is called at a very specific time, so the lock may not be necessary
        {
            competingPlayer1 = p1;
            competingPlayer2 = p2;
            prompt = category;
        }

        //now tell the main thread to switch to the game screen and populate its text fields
        AddCommand(() => {

            GameObject.Find("waitingROOM").SetActive(false);

            GameObject.Find("Player1username").GetComponent<Text>().text = competingPlayer1;
            GameObject.Find("Player2username").GetComponent<Text>().text = competingPlayer2;
            GameObject.Find("category").GetComponent<Text>().text = prompt;
        });
    }
}
