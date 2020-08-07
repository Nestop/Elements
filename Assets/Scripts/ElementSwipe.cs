using UnityEngine;
using UnityEngine.EventSystems;

public class ElementSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private int width, height, i, j;
    private float marginHorizontal, itemScale, pixelPerUnit, unitsOnHorizontal;
    private Vector2 coord;
    private Game game;

    private void Start()
    {
        width = LevelManager.instance.Width;
        height = LevelManager.instance.Height;
        marginHorizontal = LevelManager.instance.MarginHorizontal;
        unitsOnHorizontal = LevelManager.instance.UnitsOnHorizontal;

        pixelPerUnit = Screen.width / unitsOnHorizontal;
        itemScale = pixelPerUnit * (unitsOnHorizontal - marginHorizontal * 2f) / width;
        GetComponent<RectTransform>().offsetMin = new Vector2(pixelPerUnit * marginHorizontal, (Screen.height - itemScale * height) / 2f);
        GetComponent<RectTransform>().offsetMax = new Vector2(-pixelPerUnit * marginHorizontal, -(Screen.height - itemScale * height) / 2f);

        game = LevelManager.instance.GetComponent<Game>();
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        coord = eventData.position;
        float
            x = eventData.position.x,
            y = Screen.height - eventData.position.y;
        x -= marginHorizontal * pixelPerUnit;
        y -= (Screen.height - itemScale * height) / 2f;
        i = (int)(y / itemScale);
        j = (int)(x / itemScale);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - coord;
        direction.Normalize();
        int i, j;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            i = this.i;
            j = this.j + (int)(direction.x / Mathf.Abs(direction.x));
        }
        else
        {
            i = this.i - (int)(direction.y / Mathf.Abs(direction.y));
            j = this.j;
        }
        game.Swipe(this.i, this.j, i, j);
    }

}
