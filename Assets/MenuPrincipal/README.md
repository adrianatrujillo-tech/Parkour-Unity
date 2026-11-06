# Menú principal PARKOUR

Unity 6000.6.4f1. Abre `Assets/Scenes/MenuPrincipal.unity` y pulsa Play.

La imagen original compartida por la autora se conserva sin retoques en `Art/ParkourReference.png`: incluye fondo, marco, título y rótulos. Cinco objetos Button independientes se alinean con los botones dibujados; un Graphic hexagonal añade el brillo animado. Los textos de la referencia no son textos editables. Esto conserva exactamente la composición original y evita reconstruir sus adornos. Se mantiene la proporción con bandas negras en pantallas distintas de 1672:941.

Jugar y Nivel 1 cargan `Assets/Scenes/SampleScene.unity` de forma asíncrona. Niveles muestra el único nivel del proyecto; Opciones permite ajustar volumen, música y pantalla completa, guardados con claves `Parkour.Menu.*` en PlayerPrefs. Créditos y Salir tienen paneles; Salir pide confirmación y detiene Play en el editor. Escape cierra el panel o abre Salir. Flechas/teclado, ratón y mando funcionan mediante InputSystemUIInputModule; el foco vuelve al botón que abrió el panel. Los paneles bloquean la interacción con el fondo.

En `MenuPrincipal Controller`, AudioSource configura el clip de música, loop y efectos. Se reutiliza el clip existente `Assets/Audio/Hollow Knight OST - White Palace.mp3`; se puede sustituir desde el Inspector. Los sonidos de hover y clic se generan como WAV originales. Antes de distribuir el juego, revisar las licencias de música y recursos heredados.

La lista global de escenas de compilación se conserva. Para una compilación de revisión con menú inicial y nivel, ejecuta `Parkour.MenuEditor.MenuPrincipalBuilder.BuildReview` con Unity en batchmode. Escribe en `Artifacts/MenuReview/Parkour.exe` usando una lista explícita de escenas sin cambiar EditorBuildSettings. Para publicar mediante el editor, añade MenuPrincipal y SampleScene al perfil de compilación, en ese orden. Si falta el nivel, el menú muestra un mensaje recuperable.

`Parkour > Menú principal > Regenerar escena` reconstruye exclusivamente MenuPrincipal; sobrescribe los ajustes hechos manualmente en esa escena. No hace falta ejecutarlo para usar la escena entregada.

Pruebas: Test Runner > EditMode > Parkour.Menu.Tests. Incluyen integridad de imagen y enlaces, paneles en Play, restauración de foco, bloqueo modal, volumen/música y error de nivel ausente. Los tests entran y salen de Play con EnterPlayMode/ExitPlayMode.
