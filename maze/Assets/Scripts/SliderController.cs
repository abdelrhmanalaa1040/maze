using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderController : MonoBehaviour, IPointerUpHandler
{
    public Maze maze;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (maze != null)
        {
            maze.ReStartSearch();
        }
    }
}