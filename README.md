# Tactics

# ⚔️ Proyecto Tactics

Este es un proyecto personal para crear un prototipo de juego táctico inspirado en Dofus y otros juegos similares (Metal Slug Tactics, Wakfu, XCOM, FFtactics, etc).  
El proyecto está siendo desarrollado en **Unity 6.1** usando **URP** y vista **isométrica 2.5D**.

## 🛠️ Setup del Proyecto

1. Usar Unity versión **6.1 o superior**.

2. Proyecto configurado con **Universal Render Pipeline (URP)**.

3. Para ejecutar el prototipo:

    - Abrir la escena principal (`Assets/Scenes/MainScene.unity`).

    - Correr el proyecto (`Play`).



## 👨‍💻 Convenciones de Código

### 1. Nombres de Variables

- Variables privadas de instancia: prefijo `_` (underscore).

    - Ejemplo: `_renderer`, `_gridManager`

- Variables locales (dentro de métodos): **camelCase** (iniciales minusculas).

    - Ejemplo: `spawnPosition`, `tileObj`

- Propiedades públicas: **PascalCase** (iniciales mayúsculas).

    - Ejemplo: `GridPosition`, `SelectedTile`

- Escribir nombre de variables en inglés para mantener la consistencia y facilitar la colaboración internacional.

    - Ejemplo: `playerHealth` en lugar de `saludJugador`.


### 2. Métodos, Clases y funciones.

- Métodos y funciones: **PascalCase** (iniciales mayúsculas).

    - Ejemplo: `CalculateDistance`, `SpawnEnemy`

- Clases: **PascalCase** (iniciales mayúsculas).

    - Ejemplo: `PlayerController`, `EnemyAI`

- Interfaces: **IPascalCase** (iniciales mayúsculas y prefijo `I`).

    - Ejemplo: `IPlayerController`, `IEnemyAI`

### 3. Espacios y Formato

- Usar espacios en blanco para mejorar la legibilidad del código.

    - Ejemplo: `if (condition) { ... }` en lugar de `if(condition){...}`

- No es necesario el uso de {} en una sola línea o si tienen un return inmediato.

    - Ejemplo: `if (condition) DoSomething();` en lugar de `if (condition) { DoSomething(); }`

- Uso de tabs para la correcta identación del código.

    - Ejemplo: `if (condition) { ... }` debe estar indentado con tabs.

### 4. Comentarios y Documentación

- Usar comentarios para explicar el propósito de bloques de código complejos o no evidentes.

    - Ejemplo: `// Este método calcula la distancia entre dos puntos` antes de un método que realiza esa acción.

- Evitar comentarios innecesarios o redundantes.

    - Ejemplo: `// Incrementa el contador` en IncrementCounter().

- Comentarios en inglés para mantener la consistencia y facilitar la colaboración internacional.

    - Ejemplo: `// This method spawns an enemy` en lugar de `// Este método genera un enemigo`.

### 5. Buenas Prácticas

- Usar `SerializeField` para variables privadas que necesitan ser visibles en el Inspector de Unity.

    - Ejemplo: `[SerializeField] private int _health;`

- Usar `const` para valores constantes que no cambian durante la ejecución del programa.

    - Ejemplo: `const int MAX_PLAYERS = 4;`

- Usar `readonly` para variables que no deben cambiar después de la inicialización.

    - Ejemplo: `readonly int _maxHealth = 100;`

- Usar new() dentro de lo posible al crear variables, listas,etc.

    - Ejemplo: `List<int> myList = new();`

- Usar Eventos dentro de lo posible para manejar los sistema.

    - Ejemplo: `OnPlayerDie`, `OnPlayerSpawn`

- Documentar el código con comentarios claros y concisos.

    - Ejemplo: `// Este método se encarga de...`

- Usar nombres descriptivos para las variables y métodos.

    - Ejemplo: `CalculateDistance`, `SpawnEnemy`

- Evitar abreviaciones innecesarias en los nombres de variables y métodos.

    - Ejemplo: `CalculateDistance` en lugar de `CalcDist`

- Evitar referenciar en exceso a objetos de Unity (ejemplo: `GameObject`, `Transform`, `MonoBehaviour`) en los nombres de variables y métodos.

    - Entre menos se use más fácil es testear nuevos personajes, habilidades, etc.

## 🏗️ Roadmap Inicial

-  Generar grid básica

-  Detección de clicks en tiles

-  Efecto de resaltado al pasar mouse

-  Sistema de selección de tiles

-  Spawn de unidades en el grid

-  Sistema básico de turnos

-  Movimiento de unidades

-  Ataque y habilidades básicas

-  UI de combate

## 👨‍💻 Autor

- Proyecto personal de [Luis Orlando Miño Bustos]

- Proyecto no comercial, uso educativo y de experimentación.

## 📊 TODO Futuro

- Agregar sistema de pathfinding (ej: A* Pathfinding para movimiento de unidades).

- Implementar IA básica para enemigos.

- Sistema de habilidades con área de efecto (AoE).

- Sistema de estadísticas para unidades (vida, ataque, defensa, etc).

- Sistema de estados alterados (poison, stun, heal-over-time).

- Animaciones y FX básicos para habilidades.

- Guardado y carga de partidas.

- Múltiples tipos de terreno que afecten movimiento o combate.

- Mejorar UI para turnos y habilidades.