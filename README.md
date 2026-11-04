# tenis 2

Juego de tenis en VR hecho con Unity y XR Interaction Toolkit. El jugador se coloca en una pista, recibe pelotas lanzadas por generadores y las golpea con una raqueta. Solo cuentan los puntos de pelotas que hayan tocado la raqueta antes de entrar en la zona de puntuación.

## Requisitos

- **Unity** `6000.5.0f1` (o compatible con el proyecto)
- Headset VR compatible con OpenXR / XR Interaction Toolkit
- Controles (mandos); el proyecto usa el setup de manos/controllers de XRI

## Abrir el proyecto

1. Abre la carpeta del repo con Unity Hub.
2. Carga la escena principal: `Assets/Scenes/SampleScene.unity`.
3. Entra en Play con el headset conectado (o el simulador XR si lo tienes configurado).

## Controles

| Acción | Cómo |
| --- | --- |
| Agarrar raqueta | Grip / agarre XR en la raqueta |
| Dash lateral | Swipe rápido izquierda/derecha con el mando izquierdo |
| Teleport | Stick derecho (modo teleport; el movimiento suave del mando derecho debe estar desactivado) |
| Panel de control | Interactuar con la UI del panel en la pista (rayo / poke) |

El dash no atraviesa las paredes de la pista (`PlayerWalls`).

## Panel de control

En la estación de control puedes:

- **Start / End** — iniciar o parar el lanzamiento de pelotas
- **Dificultad (Fácil / Normal / Difícil)** — cambia la velocidad de la pelota vía el ángulo de lanzamiento (fácil = más lenta; difícil = más rápida). Por defecto: Fácil
- **Generadores** — cuántos orígenes lanzan pelotas (1–3). Por defecto: **3**
- **Frecuencia** — segundos entre lanzamientos. Por defecto: **3 s**
- **Regenerar raqueta** — vuelve a spawnear la raqueta en su posición inicial

## Puntuación

Una pelota solo suma punto si:

1. Ha colisionado antes con un objeto con tag `Racket`, y
2. Después entra en el collider de puntuación

## Scripts principales

| Script | Rol |
| --- | --- |
| `BallGeneratorScript` | Lanza pelotas hacia el campo rival |
| `ControlStationUI` | Cablea botones y sliders del panel |
| `BallScoreScript` | Marca si la pelota fue golpeada por la raqueta |
| `PointCounterScript` | Incrementa el marcador |
| `SwipeDash` | Dash lateral con el mando izquierdo |
| `RestrictGrabToX` | Limita el agarre de generadores al eje X |

Scripts en `Assets/Scripts/`.

## Estructura útil

```
Assets/
  Scenes/SampleScene.unity   # escena de juego
  Scripts/                   # lógica del tenis
  Samples/                   # samples de XR Interaction Toolkit / XR Hands
```

## Notas

- La raqueta debe tener el tag **`Racket`**.
- Las pelotas usan el tag **`Ball`**.
- Las paredes que limitan al jugador están en el objeto `PlayerWalls` (layer `PlayerWalls`).
