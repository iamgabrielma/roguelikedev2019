using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLogManager : MonoBehaviour
{
    public static MessageLogManager Instance;

    public readonly Queue<string> messageQueue = new Queue<string>();

    public Text textMessage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        MessageLogManager.Instance.AddToQueue("message in the queue 1!");
        MessageLogManager.Instance.AddToQueue("message in the queue 2!");
        MessageLogManager.Instance.AddToQueue("message in the queue 3!");
    }

    void Update()
    {
        if (messageQueue.Count <= 0)
        {
            return;
        }
        ExecuteQueue();


    }

    public void AddToQueue(string message)
    {
        messageQueue.Enqueue(message); // Enqueue our message to our messageQueue Queue
    }

    public void ExecuteQueue()
    {
        /* Works */
        //var value = messageQueue.Dequeue();
        //textMessage.text = value;
        //StartCoroutine(_ExecuteQueue());

        /* Testing adding several messages permanently */
        var value = messageQueue.Dequeue();
        //textMessage.text = "";
        for (int i = 0; i < messageQueue.Count+1; i++)
        {
            textMessage.text += value + "\n";
            //Debug.Log(textMessage.text);
        }

    }

    IEnumerator _ExecuteQueue() {


        yield return new WaitForSeconds(1);
    }
}
