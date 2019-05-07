using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public delegate void Command();

public class GameManager : MonoBehaviour {

    private static Queue<Command> commands; //all of the operations the game manager should perform, in order

    //game-state information:
    static string roomCode = "";
    static List<string> playerNames;
    static string gameTime = "";

    static string competingPlayer1 = "";
    static string competingPlayer2 = "";
    static string prompt = "";

    static string[] answers = { "", ""};

    static float[] voteSplit = { 0.5f};

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
        AddCommand(() => { GameObject.Find("onlinePlayers").GetComponent<Text>().text += " " + playerName + ","; });
    }

    /// <summary>
    /// Displays the tutorial timeline animation sequence and fires off start game when finished
    /// </summary>
    public static void DisplayTutorial(socketHandler handler)
    {
        AddCommand(() => {
            GameObject.Find("Canvas").transform.Find("tutorial").gameObject.SetActive(true);
            PlayableDirector director = GameObject.Find("tutorialTimeline").GetComponent<PlayableDirector>();
            director.Play();
            director.stopped += (dir) => {
                handler.EmitStartGame();
            };
        });
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

            if(GameObject.Find("waitingROOM"))
                GameObject.Find("waitingROOM").SetActive(false);

            GameObject.Find("Canvas").transform.Find("winner").gameObject.SetActive(false);

            GameObject.Find("Player1username").GetComponent<Text>().text = competingPlayer1;
            GameObject.Find("Player2username").GetComponent<Text>().text = competingPlayer2;
            GameObject.Find("category").GetComponent<Text>().text = prompt;
        });
    }

    public static void DisplayAnswer(int player, string answer)
    {
        lock (answers[player]) //only update one answer at a time, and only read when done updating
        {
            answers[player] = answer;
        }

        //now tell the main thread to switch to the game screen and populate its text fields        
        AddCommand(() => { 
            GameObject tempAns = GameObject.Find("answer" + (player + 1));
            print(tempAns);
            GameObject.Find("answer" + (player + 1)).GetComponent<Text>().text = answers[player]; 
        });
    }

    public static void UpdateTime(int time)
    {
        lock (gameTime)
        {
            gameTime = time.ToString();
        }

        //now tell the main thread to display the correct time
        AddCommand(() => { GameObject.Find("Timer").GetComponent<Text>().text = gameTime; });
    }

    public static void UpdateVote(float percent)
    {
        lock (voteSplit)
        {
            voteSplit[0] = percent;
        }

        //now tell the main thread to reposition the vote indicator
        AddCommand(() => { GameObject.Find("VoteIndicator").GetComponent<RectTransform>().anchoredPosition = new Vector3(Mathf.Lerp(-420f, 420f, percent), 176f, 0); });
    }

    public static void EndGame(int winner)
    {
        //tell the main thread to display the winner
        AddCommand(() => {

            GameObject.Find("Canvas").transform.Find("winner").gameObject.SetActive(true);
            GameObject.Find("result").GetComponent<Text>().text = (winner == 0 ? competingPlayer1 : competingPlayer2) + " has won !";
        });
    }
}
