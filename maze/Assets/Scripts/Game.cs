using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;


public interface IState
{
    public string Name { get; set; }
}

public interface IStep
{
    public string Name { get; set; }
}
public interface IGame 
{
    public IState getInitState();
    public bool isGoal( IState  state );
    public float getCost(IState state, IStep step);
    public List<IStep> getSteps(IState state);
    public IState getSuccessor(IState state, IStep step);

    
}
