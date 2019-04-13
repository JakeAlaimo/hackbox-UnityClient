using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public delegate void Command(List<string> parameters);

public class GameManager : MonoBehaviour {

    //private static Queue<Command> commands; //all of the operations the game manager should perform, in order

    //game-state information:
    static string roomCode = "";
    static List<string> playerNames;
    static float gameTime = 0;

    static string competingPlayer1 = "";
    static string competingPlayer2 = "";

    static float votePercent = 0.5f;


    // Use this for initialization
    void Start () {

        playerNames = new List<string>();

    }
	
	// Update is called once per frame
	void Update ()
    {

        //while (commands.Count > 0)
        //{
        //    //we need to lock prior to executing commands. The socket event listener threads might try to modify the queue at the same time we are executing
        //    lock (commands)
        //    {
        //        commands.Dequeue().Invoke(); //run the the next command
        //    } //unlock following the execution of a single command. Allows new commands to be added to the queue as quickly as possible
        //}

	}

    ///// <summary>
    ///// Tells the GameManager to perform a new game-related task.
    ///// </summary>
    ///// <param name="cmd">The callback function the GameManager should perform.</param>
    //public static void AddCommand(Command cmd)
    //{
    //    //we can only add to the queue while nothing else is accessing it
    //    lock (commands)
    //    {
    //        commands.Enqueue(cmd); //add the command to the queue and immediately release the lock
    //    }
    //}

    public static void DisplayRoomCode(string rmCd)
    {
        lock (roomCode) //probably won't be a factor as roomCode is only set once
        {
            roomCode = rmCd;
        }

        //now display the roomcode on the UI
        GameObject.Find("code").GetComponent<Text>().text = roomCode;
    }

    public static void AddPlayer(string playerName)
    {
        lock (playerNames) //add player only when other threads aren't reading from the list or modifying it
        {
            playerNames.Add(playerName);
        }

        //now add the player's name to the player display
        GameObject.Find("code").GetComponent<Text>().text = roomCode;
    }
}
