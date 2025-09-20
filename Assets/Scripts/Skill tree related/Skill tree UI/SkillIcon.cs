using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite lockedIconBg;
    [SerializeField] private Sprite unlockedIconBg;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void InitialiseIcon(bool isFilled)
    {
        if (isFilled)
        {
            image.sprite = unlockedIconBg;
        }
        else
        {
            image.sprite = lockedIconBg;
        }
    }
}
