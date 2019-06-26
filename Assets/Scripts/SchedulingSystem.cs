using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Allows us to use .First() method

public class SchedulingSystem
{
    private int _time;
    private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;
    // https://roguesharp.wordpress.com/2016/08/27/roguesharp-v3-tutorial-scheduling-system/
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?redirectedfrom=MSDN&view=netframework-4.8
    /*
    - Represents a collection of key/value pairs that are sorted on the key
    - "Key" in the dictionary is an integer that represents the time.
    - The "Value" of each dictionary entry is a List<IScheduleable>
    - IScheduleable: custom interface with a single property "Time". we can put anything on the schedule as long as it has a Time which represents how many turns pass until its time comes up again on the schedule.  
    - we will give each Actor a Speed statistic which will represent how often they can take actions. 
    - our scheduling system is that it will keep track of the time or number of turns that have passed as well as the current time.
    - The Speed of the actor will determine when they get inserted into the timeline. 
    - having a lower speed is better.
    - why do we have a List<> of them for each key in the dictionary? more than one Actor can act on the same time interval  
    */

    // constructor?
    public SchedulingSystem()
    {
        _time = 0;
        _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
    }

    // Adds to the schedule
    public void Add( IScheduleable scheduleable ) {

        int key = _time + scheduleable.Time;

        //if (key != null) // We cannot do ( int != null ) because always is True. We have to check if the List contains the key
        if (!_scheduleables.ContainsKey(key)) // If the List does not contains the key "key", then add it
        {
            //Debug.Log("No key: " + key); // 1
            //Debug.Log("Type: " + scheduleable.GetType()); // Type: Entity
            _scheduleables.Add(key, new List<IScheduleable>() );
            //_scheduleables.Add(key, scheduleable);
            //Debug.Log("Yes key: " + key); // 1
            //Debug.Log("Type: " + scheduleable.GetType()); // Type: Entity

        }

        // This is placing the scheduleable to our _scheduleables list, at the current time = 0 + the entity time (speed) = 1
        _scheduleables[key].Add(scheduleable);

        //foreach (var item in _scheduleables)
        //{
        //    Debug.Log("key: " + item.Key + " | value: " + item.Value[0].Time);
        //}

    }

    // Removes from the schedule
    // Also used for when an enemy is killed, removes it from the schedule before their actions trigger again
    public void Remove( IScheduleable scheduleable )
    {
        KeyValuePair<int, List<IScheduleable>> scheduleableItemFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);
        // We define a key that can be set, or retrieved
        // We set the key to -1 and the value to null, I believe to start this as "null" as possible. TODO: Question.

        foreach (var item in _scheduleables)
        {
            // We iterate through our list, and for each item there, we check if its value contains scheduleable? I believe this mean if has been added, as is used within Add() method
            if (item.Value.Contains(scheduleable))
            {
                scheduleableItemFound = item; // Add it to our list of scheduleable items found
                break;
            }
            if (scheduleableItemFound.Value != null) //If there's something in the list, and is not null
            {
                scheduleableItemFound.Value.Remove( scheduleable ); // remove it

                if (scheduleableItemFound.Value.Count <= 0) // If there's no items in our found list
                {
                    _scheduleables.Remove(scheduleableItemFound.Key); // remove the key as well
                }
            }
        }

    }

    public IScheduleable Get() {

        var firstScheduleableGroup = _scheduleables.First(); // .First() returns the first element of a sequence, in this case the 1st value-key pair from our _scheduleables List
        //Debug.Log(firstScheduleableGroup.Key + " | " + firstScheduleableGroup.Value);
        var firstScheduleable = firstScheduleableGroup.Value.First(); // The we use it again to take the 1st value of the 1st value-key pair
        //Remove(firstScheduleable); // Once we got it, we remove it from our List via the SchedulingSystem.Remove() method above //wut, el problema estaba aqui
        _time = firstScheduleableGroup.Key;

        return firstScheduleable;


    }

    // Get the current time (turn) for the schedule
    public int GetTime() {

        return _time;
    }

    // Reset the time (turn) and clear the schedule
    public void ClearSchedule() {

        _time = 0;
        _scheduleables.Clear();
    }


}
