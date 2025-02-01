using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Strategy Enum
public enum Strategy
{
    BreadthFS,
    DFS,
    UCS,
    BestFS,
    AStar
}



public interface IAlgorithm
{
    public Tuple<List<IStep>, List<IState>> Search(IGame game, IState initialState);
}


// Node class for holding state and actions
public class Node
{
    public IState State { get; set; }
    public List<IStep> steps { get; set; } = new List<IStep>();
    public int Cost { get; set; } = 0; // For UCS, BestFS, and A*
    public int Priority { get; set; } = 0; // For priority queue use

    public Node(IState state)
    {
        State = state;
    }
}

public class BreadthFS : IAlgorithm
{
    public Tuple<List<IStep>, List<IState>> Search(IGame game, IState initialState)
    {
        var queue = new Queue<Node>();
        var exploredNodes = new List<IState>();

        var explored = new HashSet<IState>();

        queue.Enqueue(new Node(initialState));


        while (queue.Any())
        {
            var currentNode = queue.Dequeue();

            if (game.isGoal(currentNode.State))
                return Tuple.Create(currentNode.steps, exploredNodes);


            if (!explored.Contains(currentNode.State))
            {
                explored.Add(currentNode.State);
                exploredNodes.Add(currentNode.State);

                foreach (var action in game.getSteps(currentNode.State))
                {
                    var successorState = game.getSuccessor(currentNode.State, action);
                    if (!explored.Contains(successorState))
                    {
                        var newNode = new Node(successorState)
                        {
                            steps = new List<IStep>(currentNode.steps) { action }
                        };
                        queue.Enqueue(newNode);
                    }
                }
            }
        }


        return Tuple.Create(new List<IStep>(), exploredNodes);
    }

}
public class  DepthFirstSearch: IAlgorithm
{
    public Tuple<List<IStep>, List<IState>> Search(IGame game, IState initialState)
    {
        var stack = new Stack<Node>();
        var exploredNodes = new List<IState>();

        var explored = new HashSet<IState>();

        stack.Push(new Node(initialState));

        while (stack.Any())
        {
            var currentNode = stack.Pop();

            if (game.isGoal(currentNode.State))
                return Tuple.Create(currentNode.steps, exploredNodes);

            if (!explored.Contains(currentNode.State))
            {
                explored.Add(currentNode.State);
                exploredNodes.Add(currentNode.State);

                foreach (var action in game.getSteps(currentNode.State))
                {
                    var successorState = game.getSuccessor(currentNode.State, action);
                    if (!explored.Contains(successorState))
                    {
                        var newNode = new Node(successorState)
                        {
                            steps = new List<IStep>(currentNode.steps) { action }
                        };
                        stack.Push(newNode);
                    }
                }
            }
        }

        return Tuple.Create(new List<IStep>(), exploredNodes);
    }
}

public class Dijkstra : IAlgorithm
{
    public Tuple<List<IStep>, List<IState>> Search(IGame game, IState initialState)
    {
        var priorityQueue = new SortedSet<(int, Node)>(Comparer<(int, Node)>.Create((a, b) =>
        {
            int compare = a.Item1.CompareTo(b.Item1);
            return compare == 0 ? a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode()) : compare;
        }));

        var exploredNodes = new List<IState>();
        var explored = new Dictionary<IState, int>();

        var startNode = new Node(initialState) { Cost = 0 };
        priorityQueue.Add((0, startNode));
        explored[initialState] = 0;

        while (priorityQueue.Any())
        {
            var (currentCost, currentNode) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            if (game.isGoal(currentNode.State))
                return Tuple.Create(currentNode.steps, exploredNodes);

            if (!exploredNodes.Contains(currentNode.State))
            {
                exploredNodes.Add(currentNode.State);
                foreach (var action in game.getSteps(currentNode.State))
                {
                    var successorState = game.getSuccessor(currentNode.State, action);
                    int stepCost = (int)game.getCost(currentNode.State, action);
                    int newCost = currentNode.Cost + stepCost;
                    if (!explored.ContainsKey(successorState) || newCost < explored[successorState])
                    {
                        var newNode = new Node(successorState)
                        {
                            steps = new List<IStep>(currentNode.steps) { action },
                            Cost = newCost
                        };

                        priorityQueue.Add((newCost, newNode));
                        explored[successorState] = newCost;
                    }
                }
            }
        }

        return Tuple.Create(new List<IStep>(), exploredNodes);
    }
}

