using UnityEngine;
using UnityEngine.UI;

public class ControlStationUI : MonoBehaviour
{
    [Header("Generador de pelotas")]
    public BallGeneratorScript ballGenerator;

    [Header("Botones de control")]
    public Button startButton;
    public Button endButton;

    [Header("Botones de dificultad")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("Sliders")]
    public Slider generatorsSlider;
    public Slider frequencySlider;

    //gris si no están apretats i blau si ho estan
    private static readonly Color UnselectedColor =
        new Color(0.14509805f, 0.15686275f, 0.1764706f, 1f);
    private static readonly Color ActiveColor = new Color32(0x00, 0xBF, 0xFF, 0xFF);

    private void Start()
    {
        if (ballGenerator == null)
        {
            Debug.LogError(
                "ControlStationUI: falta asignar BallGeneratorScript."
            );
            return;
        }

        if (generatorsSlider != null)
        {
            generatorsSlider.onValueChanged.AddListener(
                ballGenerator.SetNumberOfActiveGenerators
            );
        }

        if (frequencySlider != null)
        {
            frequencySlider.onValueChanged.AddListener(
                ballGenerator.SetLaunchInterval
            );
        }

        // Conecta los botones de inicio y parada.
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartPressed);
        }

        if (endButton != null)
        {
            endButton.onClick.AddListener(OnStopPressed);
        }

        if (easyButton != null)
        {
            easyButton.onClick.AddListener(SelectEasy);
        }

        if (normalButton != null)
        {
            normalButton.onClick.AddListener(SelectNormal);
        }

        if (hardButton != null)
        {
            hardButton.onClick.AddListener(SelectHard);
        }

        SelectEasy();
        OnStartPressed();
    }

    public void OnStartPressed()
    {
        ballGenerator.StartGeneration();
        HighlightRunControl(startButton);
    }

    public void OnStopPressed()
    {
        ballGenerator.StopGeneration();
        HighlightRunControl(endButton);
    }

    public void SelectEasy()
    {
        ApplyDifficulty(1, 3f);
        HighlightDifficulty(easyButton);
    }

    public void SelectNormal()
    {
        ApplyDifficulty(2, 1.8f);
        HighlightDifficulty(normalButton);
    }

    public void SelectHard()
    {
        ApplyDifficulty(3, 1f);
        HighlightDifficulty(hardButton);
    }

    private void ApplyDifficulty(int generators, float interval)
    {
        if (generatorsSlider != null)
        {
            generatorsSlider.value = generators;
        }

        if (frequencySlider != null)
        {
            frequencySlider.value = interval;
        }

        // Aplicación explícita para no depender de los eventos del slider.
        ballGenerator.SetNumberOfActiveGenerators(generators);
        ballGenerator.SetLaunchInterval(interval);
    }

    private void HighlightRunControl(Button selected)
    {
        SetButtonSelectedColor(startButton, startButton == selected);
        SetButtonSelectedColor(endButton, endButton == selected);
    }

    private void HighlightDifficulty(Button selected)
    {
        SetButtonSelectedColor(easyButton, easyButton == selected);
        SetButtonSelectedColor(normalButton, normalButton == selected);
        SetButtonSelectedColor(hardButton, hardButton == selected);
    }

    private static void SetButtonSelectedColor(Button button, bool selected)
    {
        if (button == null || button.targetGraphic == null)
            return;

        Color color = selected ? ActiveColor : UnselectedColor;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        button.colors = colors;
        button.targetGraphic.color = color;
    }
}
