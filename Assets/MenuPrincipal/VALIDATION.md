# Validación del menú

6 de noviembre de 2026 · Unity 6000.6.4f1.

La escena se regeneró y guardó directamente desde Unity. Se retiraron Niveles y Créditos; quedan Jugar, Opciones y Salir con texto UI real. La fuente Lato se incluye en el proyecto, con licencia OFL, para corregir los textos invisibles de los paneles. El gráfico de los botones declara explícitamente su CanvasRenderer.

Comprobaciones completadas:

- Compilación C# del runtime/jugador, runtime/editor, generador y pruebas, sin errores.
- Test Runner en el editor: 3 pruebas ejecutadas, 3 correctas, 0 fallidas. Informe local: Artifacts/MenuTests.xml.
- SceneHasThreeWiredButtonsAndEmbeddedFonts: botones, enlaces, navegación, fuente real en todos los Text y CanvasRenderer.
- PanelsBlockBackgroundAndRestoreFocus: abre/cierra Opciones y Salir en Play, comprueba acciones de entrada habilitadas, bloqueo del fondo, foco y generación de vértices de texto visibles.
- OptionsPersistAndMissingLevelShowsRecoverableMessage: volumen, música y mensaje recuperable de nivel ausente.
- Inspección visual en Play de los tres botones y ambos paneles mediante las acciones de previsualización del editor: títulos, rótulos, controles, porcentaje de volumen y acciones de salida visibles y sin solapamientos.

Los clics inyectados por el control de escritorio no activaron la UI de GameView; por ello la revisión visual se realizó mediante las acciones de previsualización. Las pruebas ejecutan los eventos de los botones, pero no sustituyen una prueba manual con ratón físico/mando.

Pendiente para aprobar el PR: probar ratón, teclado y mando físicamente, pantalla completa y compilación de jugador con MenuPrincipal y SampleScene. El generador BuildReview usa una lista explícita y no cambia EditorBuildSettings. Se conservan fuera del commit los cambios locales preexistentes y los ajustes que Unity reserializó al abrir el editor.
