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
        private BoxCollider2D swipeScreen;

        private void Start()
        {
            swipeScreen = GetComponent<BoxCollider2D>();
            game = LevelManager.instance.game;
        }

        public void UpdateSize()
        {
            float scaleX = CS.ElementScale * CS.Width;
            float scaleY = CS.ElementScale * CS.Height;
            swipeScreen.size = new Vector2(scaleX, scaleY);
            swipeScreen.offset = new Vector2(0f, CS.StartSpawnPoint.y - CS.ElementScale*(CS.Height-1)/2f);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            beginDragPos = eventData.position;
            float x = beginDragPos.x;
            float y = Screen.height - beginDragPos.y;
            x -= (Screen.width - CS.ElementPixelScale*CS.Width)/2f;
            y -= (Screen.height - CS.GroundLevel*CS.PixelsPerUnit - CS.ElementPixelScale*CS.Height);
            i  = (int)(y / CS.ElementPixelScale);
            j  = (int)(x / CS.ElementPixelScale);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(this.i >= CS.Height || this.j >= CS.Width) return;

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
