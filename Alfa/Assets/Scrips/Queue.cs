using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Queue
{
    private List<RoomSpawner> queue;

    public Queue(RoomSpawner first)
    {
        queue = new List<RoomSpawner>();
        queue.Add(first);
    }

    public bool isEmpty()
    {
        if (queue.Count == 0)
        {
            return true;
        }
        else return false;
    }

    public void emptyQueue()
    {
        queue.Clear();
    }

    public bool contains(RoomSpawner spawn)
    {
        if (queue.Contains(spawn))
            return true;
        else return false;
    }

    public void enqueue(RoomSpawner item)
    {
        queue.Add(item);
    }

    public RoomSpawner dequeue()
    {
        var first = queue.First();
        queue.RemoveAt(0);
        return first;
    }
}
