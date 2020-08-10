using UnityEngine;
using GameGUI;

namespace GameLogic
{
    [RequireComponent(typeof(Game))]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance = null;
        public Game game;

        public int Width, Height;
        public Sprite Background;
        public GameObject GameEffect;
        public int[] ElementsID;
        public float MarginHorizontal = 0.5f;
        public int Steps = 1;

        private float unitsOnHorizontal;
        private float unitsOnVertical;
        private float elementScale;

        public float UnitsOnHorizontal => unitsOnHorizontal;
        public float ElementScale => elementScale;
        

        private void Start()
        {
            if (instance == null || instance.Equals(this))
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }

            game = GetComponent<Game>();
        }

        public void LoadLevel(int level)
        {
            GUIManager.instance.LoadLevel();
            GameManager.instance.Background.sprite = Background;
            unitsOnHorizontal =
                GameManager.instance.MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x -
                GameManager.instance.MainCamera.ScreenToWorldPoint(Vector3.zero).x;
            unitsOnVertical = 
                GameManager.instance.MainCamera.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f)).y -
                GameManager.instance.MainCamera.ScreenToWorldPoint(Vector3.zero).y;
            elementScale = (unitsOnHorizontal - MarginHorizontal * 2f) / Width;
            float ground = (unitsOnVertical/2f)/5f;
            float x, y;
            Vector2 position;
            GameObject Element;
            Element[,] elements = new Element[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    int id = Width * i + j;
                    if (ElementsID[id] == -1) continue;

                    x = MarginHorizontal - unitsOnHorizontal / 2f + elementScale / 2f + j * elementScale;
                    y = (Height - 1) * elementScale / 2f - i * elementScale;
                    position = new Vector2(x, y);
                    Element = new GameObject("Element", typeof(SpriteRenderer), typeof(Animator));
                    Element.transform.localScale = Vector2.one * elementScale;
                    Element.transform.localPosition = position;
                    SpriteRenderer sprRend = Element.GetComponent<SpriteRenderer>();
                    sprRend.sortingOrder = -(Width * i - j);
                    Animator animator = Element.GetComponent<Animator>();
                    animator.runtimeAnimatorController = GameManager.instance.Elements[ElementsID[id]].Controller;
                    animator.SetFloat("Random",Random.Range(0f,1f));
                    elements[i, j] = new Element(ElementsID[id], Element.transform, sprRend);
                }
            game.GetElements(elements);
        }
    }
}
