
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Etiquetas de los orígenes")]
    public TMP_Text origin1LabelGenerador;
    public TMP_Text origin2LabelGenerador;
    public TMP_Text origin3LabelGenerador;

    [Header("Colores")]
    public Color selectedColor = new Color32(0, 191, 255, 255); // Azul
    public Color normalColor = new Color32(255, 255, 255, 255); // Blanco

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
        UpdateOriginLabels(value);
    }

    private void UpdateOriginLabels(float selectedOrigin)
    {
        if (origin1LabelGenerador != null)
            origin1LabelGenerador.color = selectedOrigin == 1
                ? selectedColor : normalColor;

        if (origin2LabelGenerador != null)
            origin2LabelGenerador.color = selectedOrigin == 2
                ? selectedColor : normalColor;

        if (origin3LabelGenerador != null)
            origin3LabelGenerador.color = selectedOrigin == 3
                ? selectedColor : normalColor;
    }
}
