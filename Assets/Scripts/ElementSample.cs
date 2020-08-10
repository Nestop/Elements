using UnityEngine;

[CreateAssetMenu(fileName = "New Element", menuName = "Element Sample", order = 51)]
public class ElementSample : ScriptableObject
{
    [SerializeField] private Texture2D icon = default;
    public Texture2D Icon => icon;
    [SerializeField] private RuntimeAnimatorController controller = default;
    public RuntimeAnimatorController Controller => controller;
}
