
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

    private void Start()
    {
        if (ballGenerator == null)
        {
            Debug.LogError(
                "ControlStationUI: falta asignar BallGeneratorScript."
            );
            return;
        }

        // Conecta los sliders con el generador.
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
            startButton.onClick.AddListener(
                ballGenerator.StartGeneration
            );
        }

        if (endButton != null)
        {
            endButton.onClick.AddListener(
                ballGenerator.StopGeneration
            );
        }

        // Conecta los botones de dificultad.
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

        // Configuración inicial: Easy.
        SelectEasy();

        // Comienza la generación automáticamente.
        ballGenerator.StartGeneration();
    }

    public void SelectEasy()
    {
        ApplyDifficulty(1, 3f);
    }

    public void SelectNormal()
    {
        ApplyDifficulty(2, 1.8f);
    }

    public void SelectHard()
    {
        ApplyDifficulty(3, 1f);
    }

    // Actualiza los sliders y aplica los valores al generador.
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
}
