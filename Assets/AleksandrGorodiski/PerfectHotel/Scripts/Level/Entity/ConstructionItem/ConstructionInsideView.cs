using UnityEngine;

public class ConstructionInsideView : ConstructionItemView
{
    [SerializeField] private Sprite[] _icons;
    public Sprite[] Icons => _icons;
}
