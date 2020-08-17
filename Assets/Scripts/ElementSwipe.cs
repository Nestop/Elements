using UnityEngine;
using UnityEngine.EventSystems;

using CS = GameLogic.CoordinateSystem;

namespace GameLogic
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ElementSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private int i, j;
        private Vector2 beginDragPos;
        private Game game;

        private void Start()
        {
            BoxCollider2D swipeScreen = GetComponent<BoxCollider2D>();
            float scaleX = CS.ElementScale * CS.Width;
            float scaleY = CS.ElementScale * CS.Height;
            swipeScreen.size = new Vector2(scaleX, scaleY);

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
            x -= CS.MarginHorizontal * CS.PixelsPerUnit;
            y -= (Screen.height - CS.ElementPixelScale * CS.Height) / 2f;
            i  = (int)(y / CS.ElementPixelScale);
            j  = (int)(x / CS.ElementPixelScale);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - beginDragPos;
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
