using UnityEngine;

using GameGUI;
using CS = GameLogic.CoordinateSystem;

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
            CS.SetCoordinateSystem(Width, Height, MarginHorizontal);
            GUIManager.instance.LoadLevel();
            GameManager.instance.Background.sprite = Background;
            GameObject Element;
            Element[,] elements = new Element[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    int id = Width * i + j;
                    if (ElementsID[id] == -1) continue;

                    Element = new GameObject("Element", typeof(SpriteRenderer), typeof(Animator));
                    Element.transform.localScale = Vector2.one * CS.ElementScale;
                    Element.transform.localPosition = CS.GetPosition(i,j);
                    SpriteRenderer sprRend = Element.GetComponent<SpriteRenderer>();
                    sprRend.sortingOrder = CS.GetSortingOrder(i, j);
                    Animator animator = Element.GetComponent<Animator>();
                    animator.runtimeAnimatorController = GameManager.instance.Elements[ElementsID[id]].Controller;
                    animator.SetFloat("Random",Random.Range(0f,1f));
                    elements[i, j] = new Element(ElementsID[id], Element.transform, sprRend);
                }
            game.GetElements(elements);
        }
    }
}
