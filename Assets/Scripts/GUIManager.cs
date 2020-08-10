using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
    public class GUIManager : MonoBehaviour
    {
        public static GUIManager instance = null;

        [SerializeField] private GameObject menu = default;
        [SerializeField] private GameObject game = default;
        [SerializeField] private GameObject ResultPanel = default;


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

            Initialize();
        }

        private void Initialize()
        {
            menu.SetActive(true);
            ResultPanel.SetActive(false);
            game.SetActive(false);
        }

        public void LoadLevel()
        {
            menu.SetActive(false);
            ResultPanel.SetActive(false);
            game.SetActive(true);
        }

        public void Win(bool isWin)
        {
            ResultPanel.SetActive(true);
            Text message = ResultPanel.transform.Find("Message").GetComponent<Text>();
            if (isWin)
            {
                message.text = "Вы победили!";
            }
            else
            {
                message.text = "Попробуй ещё раз";
            }
        }
    }
}