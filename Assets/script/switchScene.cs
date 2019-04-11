using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class switchScene : MonoBehaviour {


// Use this for initialization
void Start () {
        Debug.Log("game is start");


        var startBtn = gameObject.GetComponent<Button>();


        if (startBtn.name == "Btn_start")
        {
            // startBtn.ButtonClickEvent = startMethod;
        }
}


    public void startMethod(BaseEventData eventData)
    {
        Debug.Log("start to switchScene");
        Application.LoadLevelAsync("GameScene");


        Debug.LogWarning("success");


    }




    void Awake() {
        Debug.Log(" this is LoginMgr's Awake()");
    }

// Update is called once per frame
void Update () {
        Debug.Log(" this is LoginMgr's Update()");
}




 
}