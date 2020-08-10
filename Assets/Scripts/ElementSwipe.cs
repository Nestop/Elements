using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    public class ElementSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private int width, height, i, j;
        private float marginHorizontal, itemScale, pixelPerUnit, unitsOnHorizontal;
        private Vector2 beginDragPos;
        private Game game;

        private void Start()
        {
            width = LevelManager.instance.Width;
            height = LevelManager.instance.Height;
            marginHorizontal = LevelManager.instance.MarginHorizontal;
            unitsOnHorizontal = LevelManager.instance.UnitsOnHorizontal;

            pixelPerUnit = Screen.width / unitsOnHorizontal;
            itemScale = pixelPerUnit * (unitsOnHorizontal - marginHorizontal * 2f) / width;
            RectTransform swipeScreen = GetComponent<RectTransform>();
            float offsetX = pixelPerUnit * marginHorizontal;
            float offsetY = (Screen.height - itemScale * height) / 2f;
            swipeScreen.offsetMin = new Vector2( offsetX, offsetY);
            swipeScreen.offsetMax = new Vector2(-offsetX,-offsetY);

            game = LevelManager.instance.game;
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            beginDragPos = eventData.position;
            float x = beginDragPos.x;
            float y = Screen.height - beginDragPos.y;
            x -= marginHorizontal * pixelPerUnit;
            y -= (Screen.height - itemScale * height) / 2f;
            i  = (int)(y / itemScale);
            j  = (int)(x / itemScale);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - beginDragPos;
            direction.Normalize();
            int i, j;
            float lengthX = Mathf.Abs(direction.x);
            float lengthY = Mathf.Abs(direction.y);
            if (lengthX > lengthY)
            {
                i = this.i;
                j = this.j + (int)(direction.x / lengthX);
            }
            else
            {
                i = this.i - (int)(direction.y / lengthY);
                j = this.j;
            }
            game.Swipe(this.i, this.j, i, j);
        }

    }
}
