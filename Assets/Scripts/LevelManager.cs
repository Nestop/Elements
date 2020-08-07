using UnityEngine;

[RequireComponent(typeof(Game))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    public int Width, Height;
    public Sprite Background;
    public GameObject GameEffects;
    public int[] ElementsID;
    public float MarginHorizontal = 0.5f;
    public int Steps = 1;

    private float itemScale, unitsOnHorizontal;
    public float ItemScale => itemScale;
    public float UnitsOnHorizontal => unitsOnHorizontal;

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

    public void LoadLevel(int level)
    {
        GUIManager.instance.LoadLevel();
        GameManager.instance.Background.sprite = Background;
        unitsOnHorizontal =
            GameManager.instance.MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x -
            GameManager.instance.MainCamera.ScreenToWorldPoint(Vector3.zero).x;
        itemScale = (unitsOnHorizontal - MarginHorizontal * 2f) / Width;
        float x, y;
        Vector2 coord;
        GameObject Element;
        Element[,] elements = new Element[Height, Width];
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                if (ElementsID[Width * i + j] == -1) continue;

                x = MarginHorizontal - unitsOnHorizontal / 2f + itemScale / 2f + j * itemScale;
                y = (Height - 1) * itemScale / 2f - i * itemScale;
                coord = new Vector2(x, y);
                Element = new GameObject("Element", typeof(SpriteRenderer), typeof(Animator));
                Element.transform.localScale = Vector2.one * itemScale;
                Element.transform.localPosition = coord;
                SpriteRenderer sprRend = Element.GetComponent<SpriteRenderer>();
                sprRend.sortingOrder = -(Width * i - j);
                Element.GetComponent<Animator>().runtimeAnimatorController = GameManager.instance.Elements[ElementsID[Width * i + j]].Controller;
                elements[i, j] = new Element(ElementsID[Width * i + j], Element.transform, sprRend);
            }
        GetComponent<Game>().GetElements(elements);
    }
}
