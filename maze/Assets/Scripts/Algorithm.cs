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

