using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLogManager : MonoBehaviour
{
    public static MessageLogManager Instance;

    public readonly Queue<string> messageQueue = new Queue<string>();

    public Text textMessage;
    public Text restOfTexttMessage;
    private static readonly int __maxNumberOfMessagesInTheLog = 25;

    private string[] _foo;
    private List<string> _fooList = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //MessageLogManager.Instance.AddToQueue("Welcome Rogue...");
        //MessageLogManager.Instance.AddToQueue("Welcome Rogue 2...");

    }

    void Update()
    {
        //if (messageQueue.Count <= 0)
        //{
        //    return;
        //}


    }
    //TODO: Add message color
    public void AddToQueue(string message)
    {
        messageQueue.Enqueue(message); // Enqueue our message to our messageQueue Queue

        if (_fooList.Count >= __maxNumberOfMessagesInTheLog)
        {
            // Will not work, again same issue that in the past: A List of structs cannot be updated like that - the reason is that structs are copied by value and are not a reference 
            // https://answers.unity.com/questions/318922/why-cant-i-directly-change-the-value-of-a-list-ele.html
            // Solution: When working with structs you need to get the contents of the list item - modify it and then put it back again.

            _fooList.Clear(); // TODO: There's no need for thus thanks to using Insert() and not Add(). But left here for the moment.

        }
        else
        {
            //_fooList.Add(message);
            _fooList.Insert(0, message); // Using Insert() instead of Add() adds this at the beginning of the list, making the most recent message to appear first, which is what we want
        }


        ExecuteQueue();
        //Debug.Log("foolist count " + _fooList.Count.ToString());

    }

    public void ExecuteQueue()
    {
        string _bar = messageQueue.Dequeue();
        string _restOfText = "";

        for (int i = 1; i < _fooList.Count; i++) // Here we use i=1 instead of 1=0 as the first message already appears on _bar, that way the rest of the text is only that, and not the current message.
        {
            _restOfText += _fooList[i] + "\n";
        }
        // textMessage displays the last Message passed to the Queue
        textMessage.text = _bar + "\n";
        // restOfTexttMessage displays an historic of the last 25 Messages passed to the Queue, then is reset
        restOfTexttMessage.text = _restOfText;// TODO: Instead of adding it at the end of the list, maybe can be added at the beginning. Possibly via the Insert() list method

        //Debug.Log("Breakpoint: " + "queue: " + messageQueue.Count + "foo: " + _fooList.Count);
    }

}
