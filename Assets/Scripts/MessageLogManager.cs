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
    private static readonly int __maxNumberOfMessagesInTheLog = 10;

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

    public void AddToQueue(string message)
    {
        messageQueue.Enqueue(message); // Enqueue our message to our messageQueue Queue

        if (_fooList.Count >= 25)
        {
            // Will not work, again same issue that in the past: A List of structs cannot be updated like that - the reason is that structs are copied by value and are not a reference 
            // https://answers.unity.com/questions/318922/why-cant-i-directly-change-the-value-of-a-list-ele.html
            // Solution: When working with structs you need to get the contents of the list item - modify it and then put it back again.

            _fooList.Clear();

        }
        else
        {
            _fooList.Add(message);
        }


        ExecuteQueue();
        //Debug.Log("foolist count " + _fooList.Count.ToString());

    }

    public void ExecuteQueue()
    {
        string _bar = messageQueue.Dequeue();
        string _restOfText = "";

        for (int i = 0; i < _fooList.Count; i++)
        {
            _restOfText += _fooList[i] + "\n";
        }
        restOfTexttMessage.text = _restOfText;

        textMessage.text = _bar + "\n";

        Debug.Log("Breakpoint: " + "queue: " + messageQueue.Count + "foo: " + _fooList.Count);
    }

}
