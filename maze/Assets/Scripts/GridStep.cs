using System.Collections;
using System.Collections.Generic;
public enum DIRECTION
{
    UP, DOWN, LEFT, RIGHT
}
public class GridStep : IStep
{
    public int dx, dy;
    public string Name { get; set; }

    public GridStep(DIRECTION direction)
    {
        if (direction == DIRECTION.UP)
        {
            dx = 0;
            dy = 1;
            Name = "UP";
        }
        else if (direction == DIRECTION.DOWN)
        {
            dx = 0;
            dy = -1;
            Name = "DOWN";
        }
        else if (direction == DIRECTION.RIGHT)
        {
            dx = 1;
            dy = 0;
            Name = "RIGHT";
        }
        else if (direction == DIRECTION.LEFT)
        {
            dx = -1;
            dy = 0;
            Name = "LEFT";
        }
    }
}
