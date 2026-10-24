
using UnityEngine;
using UnityEngine.UI;

public class ControlStationUI : MonoBehaviour
{
    [Header("Generador de pelotas")]
    public BallGeneratorScript ballGenerator;

    [Header("Controles UI")]
    public Button startButton;
    public Button endButton;
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public Slider originSlider;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (endButton != null)
            endButton.onClick.AddListener(StopGame);

        if (easyButton != null)
            easyButton.onClick.AddListener(SelectEasy);

        if (normalButton != null)
            normalButton.onClick.AddListener(SelectNormal);

        if (hardButton != null)
            hardButton.onClick.AddListener(SelectHard);

        if (originSlider != null)
        {
            originSlider.minValue = 1;
            originSlider.maxValue = 3;
            originSlider.wholeNumbers = true;

            originSlider.onValueChanged.AddListener(
                OnOriginChanged
            );

            OnOriginChanged(originSlider.value);
        }
    }

    private void StartGame()
    {
        if (ballGenerator == null) return;

        ballGenerator.StartGeneration();
    }

    private void StopGame()
    {
        if (ballGenerator == null) return;

        ballGenerator.StopGeneration();
    }

    private void SelectEasy()
    {
        if (ballGenerator == null) return;

        ballGenerator.SetEasy();
    }

    private void SelectNormal()
    {
        if (ballGenerator == null) return;

        ballGenerator.SetNormal();
    }

    private void SelectHard()
    {
        if (ballGenerator == null) return;

        ballGenerator.SetHard();
    }

    private void OnOriginChanged(float value)
    {
        if (ballGenerator == null) return;

        ballGenerator.SetSelectedOrigin(value);
    }
}
