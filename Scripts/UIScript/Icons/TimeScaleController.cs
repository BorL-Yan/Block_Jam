using UnityEngine;
using UnityEngine.UI; // Անհրաժեշտ է Slider-ի հետ աշխատելու համար
using TMPro; // Եթե ցանկանում եք նաև թիվը ցույց տալ

public class TimeScaleController : MonoBehaviour
{
    [SerializeField] private Slider _timeSlider;
    //[SerializeField] private TextMeshProUGUI _valueText; // Պարտադիր չէ

    private void Start()
    {
        // Կարգավորում ենք սլայդերի սահմանները
        _timeSlider.minValue = 0f;    // Դադար (Pause)
        _timeSlider.maxValue = 1f;    // 3 անգամ արագացում
        _timeSlider.value = 0.4f;       // Սկզբնական արժեքը (նորմալ արագություն)
        UpdateTimeScale(_timeSlider.value);
        // Լսում ենք սլայդերի փոփոխությունը
        _timeSlider.onValueChanged.AddListener(UpdateTimeScale);
    }

    public void UpdateTimeScale(float value)
    {
        // Փոխում ենք խաղի ժամանակի արագությունը
        Time.timeScale = value;

        // Թարմացնում ենք տեքստը (եթե կա)
        // if (_valueText != null)
        // {
        //     _valueText.text = $"Speed: {value:F1}x";
        // }
    }

    // Լավ պրակտիկա է նաև թարմացնել fixedDeltaTime-ը ֆիզիկայի սահունության համար
    private void OnDisable()
    {
        Time.timeScale = 1f; // Վերադարձնում ենք նորմալ վիճակի սկրիպտը անջատելիս
    }
}