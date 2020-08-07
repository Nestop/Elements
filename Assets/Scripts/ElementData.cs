using UnityEngine;

[CreateAssetMenu(fileName = "New Element", menuName = "Element Data", order = 51)]
public class ElementData : ScriptableObject
{
    [SerializeField] private Sprite icon = default;
    public Sprite Icon => icon;
    [SerializeField] private RuntimeAnimatorController controller = default;
    public RuntimeAnimatorController Controller => controller;
}
