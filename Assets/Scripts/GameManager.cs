using UnityEngine;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        
        public Camera MainCamera => mainCamera;
        public ElementSample[] Elements => elements;
        public SpriteRenderer Background => background;
        public ElementSwipe SwipeScreen => swipeScreen;

        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private ElementSample[] elements = new ElementSample[0];
        [SerializeField] private SpriteRenderer background = default;
        [SerializeField] private ElementSwipe swipeScreen = default;

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
        }

        public static GameManager Get()
        {
            GameObject obj = GameObject.Find("GameManager");
            if (obj == null)
            {
                instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
                return instance;
            }
            else
            {
                instance = obj.GetComponent<GameManager>();
                return instance;
            }
        }
    }
}
