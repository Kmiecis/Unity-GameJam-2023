using UnityEngine;
using UnityEngine.UI;

public class UIReloadBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    public void UpdateReloadSlider(float current, float target)
    {
        slider.maxValue = target;
        slider.value = current;
    }
}