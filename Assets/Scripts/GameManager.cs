using UnityEngine;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        [SerializeField] private Camera mainCamera = default;
        public Camera MainCamera { get { return mainCamera; } }//public Camera MainCamera => mainCamera;
        [SerializeField] private ElementSample[] elements = new ElementSample[0];
        public ElementSample[] Elements { get { return elements; } }//public ElementData[] Elements => elements;
        public SpriteRenderer Background;

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
