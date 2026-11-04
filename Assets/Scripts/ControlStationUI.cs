using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlStationUI : MonoBehaviour
{
    [Header("Generador de pelotas")]
    public BallGeneratorScript ballGenerator;

    [Header("Botones de control")]
    public Button startButton;
    public Button endButton;
    public Button regenerateRacket;

    [Header("Raqueta")]
    [Tooltip("Raqueta actual en la escena.")]
    public GameObject racket;

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

    private GameObject racketPrototype;
    private Vector3 racketSpawnPosition;
    private Quaternion racketSpawnRotation;
    private Vector3 racketSpawnScale;
    private Transform racketSpawnParent;
    private string racketName = "RacquetMario";
    private Coroutine regenerateFlashRoutine;

    private void Start()
    {
        CacheRacketSpawn();

        if (ballGenerator == null)
        {
            Debug.LogError(
                "ControlStationUI: falta asignar BallGeneratorScript."
            );
        }
        else
        {
            if (generatorsSlider != null)
            {
                generatorsSlider.onValueChanged.AddListener(
                    ballGenerator.SetNumberOfActiveGenerators
                );
                ballGenerator.SetNumberOfActiveGenerators(
                    generatorsSlider.value
                );
            }

            if (frequencySlider != null)
            {
                frequencySlider.onValueChanged.AddListener(
                    ballGenerator.SetLaunchInterval
                );
                ballGenerator.SetLaunchInterval(frequencySlider.value);
            }
        }

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);

        if (endButton != null)
            endButton.onClick.AddListener(OnStopPressed);

        if (regenerateRacket != null)
            regenerateRacket.onClick.AddListener(OnRegenerateRacketPressed);

        if (easyButton != null)
            easyButton.onClick.AddListener(SelectEasy);

        if (normalButton != null)
            normalButton.onClick.AddListener(SelectNormal);

        if (hardButton != null)
            hardButton.onClick.AddListener(SelectHard);

        SetupUiRaySlider(generatorsSlider);
        SetupUiRaySlider(frequencySlider);

        if (ballGenerator != null)
        {
            SelectEasy();
            OnStopPressed();
        }
    }

    // UI + rayo: select en barra/handle y arrastrar; el drag se mantiene al pulsar.
    private static void SetupUiRaySlider(Slider slider)
    {
        if (slider == null)
            return;

        slider.interactable = true;

        if (slider.handleRect != null
            && slider.handleRect.TryGetComponent(out Graphic handleGraphic))
        {
            handleGraphic.raycastTarget = true;
            handleGraphic.raycastPadding = new Vector4(30f, 30f, 30f, 30f);
        }

        Transform background = slider.transform.Find("Background");
        if (background != null
            && background.TryGetComponent(out Graphic backgroundGraphic))
        {
            backgroundGraphic.raycastTarget = true;
            backgroundGraphic.raycastPadding = new Vector4(0f, 24f, 0f, 24f);
        }
    }

    private void OnDestroy()
    {
        if (racketPrototype != null)
            Destroy(racketPrototype);
    }

    private void CacheRacketSpawn()
    {
        if (racket == null)
            return;

        racketName = racket.name;
        racketSpawnPosition = racket.transform.position;
        racketSpawnRotation = racket.transform.rotation;
        racketSpawnScale = racket.transform.localScale;
        racketSpawnParent = racket.transform.parent;

        // Copia inactiva: al destruir la raqueta en juego no perdemos el modelo.
        racketPrototype = Instantiate(racket);
        racketPrototype.name = racketName + " (Prototype)";
        racketPrototype.SetActive(false);
        racketPrototype.transform.SetParent(null, true);
    }

    public void OnStartPressed()
    {
        if (ballGenerator == null)
            return;

        ballGenerator.StartGeneration();
        HighlightRunControl(startButton);
    }

    public void OnStopPressed()
    {
        if (ballGenerator == null)
            return;

        ballGenerator.StopGeneration();
        HighlightRunControl(endButton);
    }

    public void OnRegenerateRacketPressed()
    {
        if (racketPrototype == null)
        {
            Debug.LogWarning(
                "ControlStationUI: no hay prototipo de raqueta."
            );
            return;
        }

        if (racket != null)
            Destroy(racket);

        racket = Instantiate(
            racketPrototype,
            racketSpawnPosition,
            racketSpawnRotation,
            racketSpawnParent
        );
        racket.name = racketName;
        racket.tag = "Racket";
        racket.transform.localScale = racketSpawnScale;
        racket.SetActive(true);

        if (racket.TryGetComponent(out Rigidbody body))
        {
            body.isKinematic = false;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        if (regenerateFlashRoutine != null)
            StopCoroutine(regenerateFlashRoutine);
        regenerateFlashRoutine = StartCoroutine(FlashRegenerateButton());
    }

    private IEnumerator FlashRegenerateButton()
    {
        SetButtonSelectedColor(regenerateRacket, true);
        yield return new WaitForSeconds(0.25f);
        SetButtonSelectedColor(regenerateRacket, false);
        regenerateFlashRoutine = null;
    }

    public void SelectEasy()
    {
        if (ballGenerator != null)
            ballGenerator.SetEasy();
        HighlightDifficulty(easyButton);
    }

    public void SelectNormal()
    {
        if (ballGenerator != null)
            ballGenerator.SetNormal();
        HighlightDifficulty(normalButton);
    }

    public void SelectHard()
    {
        if (ballGenerator != null)
            ballGenerator.SetHard();
        HighlightDifficulty(hardButton);
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
