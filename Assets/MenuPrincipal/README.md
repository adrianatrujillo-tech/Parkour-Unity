# Menú principal PARKOUR

Unity 6000.6.4f1. Abre `Assets/Scenes/MenuPrincipal.unity` y pulsa Play.

El menú tiene tres botones UI: Jugar, Opciones y Salir. Se retiraron Niveles y Créditos de la escena y del fondo. `Art/ParkourReference.png` conserva la referencia original; `Art/ParkourBackground.png` es una edición sin botones realizada con image_gen integrada. El título, marco y paisaje se mantienen en el fondo; los rótulos de los botones son textos UI editables.

Opciones incluye volumen con porcentaje, música y pantalla completa. Guarda preferencias con claves `Parkour.Menu.*`. Salir ofrece Seguir jugando y Salir del juego. Escape cierra el panel o abre Salir. Los paneles bloquean el fondo y restauran el foco. La fuente Lato se incluye como recurso real con su licencia OFL para evitar textos invisibles por referencias a fuentes integradas.

Jugar carga `Assets/Scenes/SampleScene.unity` de forma asíncrona. El AudioSource de `MenuPrincipal Controller` permite sustituir la música existente. Revisar licencias de recursos heredados antes de distribuir.

`Parkour > Menú principal > Regenerar escena` reconstruye únicamente MenuPrincipal y sobrescribe los cambios manuales de esa escena. `Ejecutar pruebas` guarda el informe en `Artifacts/MenuTests.xml`. Las acciones de previsualización muestran los paneles durante Play.

`Parkour.MenuEditor.MenuPrincipalBuilder.BuildReview` compila MenuPrincipal y SampleScene en `Artifacts/MenuReview/Parkour.exe` sin cambiar EditorBuildSettings. Para compilar normalmente, añade ambas escenas al perfil en ese orden. Si falta el nivel, se muestra un mensaje recuperable.
