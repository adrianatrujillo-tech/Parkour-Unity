# Validación para revisión

Fecha: 6 de noviembre de 2026.

Comprobaciones completadas:

- Compilación C# con Roslyn y ensamblados de Unity 6000.6.4f1: runtime de jugador, runtime con UNITY_EDITOR, generador de escena y ensamblado de pruebas. Sin errores. Las pruebas utilizan algunas búsquedas de objetos que Unity 6.6 marca como obsoletas pero conserva compatibles.
- Análisis YAML: 249 documentos, 57 GameObjects, 12 Buttons con eventos persistentes (cinco principales y siete dentro de paneles), sin referencias locales colgantes y con GUID externos resueltos.
- Los cinco paneles comienzan desactivados. Los botones principales apuntan a Play, ShowLevels, ShowOptions, ShowCredits y ShowExit. SampleScene existe y es el destino configurado.
- El PNG de 1672 × 941 es idéntico byte por byte a la imagen adjunta recuperada de la conversación. No se ha recreado ni retocado.
- El único archivo preexistente modificado es Assets/Scenes/MenuPrincipal.unity. Las otras escenas, ProjectSettings y Packages permanecen intactos.
- git diff --check pasa.

Pendiente antes de aprobar el PR:

El editor instalado se detuvo durante el inicio de Unity Licensing en este entorno restringido, con errores de acceso a información del sistema. Por ello no se ejecutaron las pruebas dentro del editor, la compilación de jugador ni la inspección visual de la escena. La escena se serializó sin ejecutar Unity; el generador C# incluido permite regenerarla desde el editor si fuera necesario. La compilación C# y el análisis YAML no sustituyen esta validación.

1. Abrir el proyecto con Unity 6000.6.4f1 y esperar la importación. Comprobar que no hay errores ni scripts ausentes.
2. Abrir Assets/Scenes/MenuPrincipal.unity y pulsar Play. Revisar la composición a 16:9, 4:3 y ultrapanorámica, los cinco hover, las flechas/Enter, ratón y mando.
3. Abrir/cerrar Niveles, Opciones, Créditos y Salir, comprobar el bloqueo del fondo, Escape y la restauración de foco. Cambiar volumen/música y reiniciar Play para verificar persistencia; comprobar pantalla completa en el jugador.
4. Ejecutar Test Runner > EditMode > Parkour.Menu.Tests. Hay una prueba estructural y dos pruebas que entran en Play.
5. Compilar mediante Parkour.MenuEditor.MenuPrincipalBuilder.BuildReview, que utiliza menú y SampleScene sin cambiar la lista global de escenas. Abrir el jugador, comprobar que Jugar y Nivel 1 cargan SampleScene y que Salir cierra el jugador. Verificar el mensaje recuperable si falta un nivel en un perfil personalizado.

API de carga consultada: https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/scenemanagement/scenemanager/loadsceneasync
