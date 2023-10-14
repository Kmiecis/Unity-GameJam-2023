using UnityEngine;
using UnityEngine.UI;

public class UIReloadBar : MonoBehaviour
{
    [SerializeField] private Image image;
    
    public void UpdateReloadSlider(float current, float target)
    {
        image.fillAmount = current / target;
    }
}