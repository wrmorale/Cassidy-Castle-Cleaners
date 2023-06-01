using UnityEngine;
using UnityEngine.UI;

public class TutorialControls : MonoBehaviour
{
    public Sprite[] images;
    public Image displayImage;

    private int currentIndex = 0;

    private void Start()
    {
        if (displayImage != null && images.Length > 0)
        {
            displayImage = GetComponent<Image>();
            displayImage.sprite = images[currentIndex];
        }
    }

    public void NextImage()
    {
        currentIndex++;

        if (currentIndex >= images.Length)
        {
            displayImage.enabled = false;
        }   

        if (displayImage != null)
        {
            displayImage.sprite = images[currentIndex];
        }
    }
}