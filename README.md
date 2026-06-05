# 🎮 RogueLiteDemo - Manual de Usuario y Guía de Ejecución

¡Bienvenido al repositorio oficial de **RogueLiteDemo**! Este proyecto es un juego desarrollado en **Unity 6 (6000.4.2f1)** que integra bases de datos en tiempo real y sistemas de autenticación mediante **Firebase (Auth y Firestore)**.

---

## 🚀 Requisitos del Sistema
* **Plataforma:** Windows (Arquitectura x64).
* **Entorno de Red:** Requiere conexión a Internet activa para la sincronización inicial con los servicios de Google Firebase (*ver sección de Modo Seguro*).
* **Periféricos soportados:** Teclado y Ratón / Mando de Consola (Xbox, PlayStation o compatible).

---

## 🕹️ Mapeado de Controles (Sistemas de Entrada)

El juego cuenta con soporte híbrido completo. Puedes jugar utilizando el combo clásico de teclado o conectar un mando en cualquier momento de la partida.

### ⌨️ Configuración: Teclado y Ratón
* **Movimiento del personaje:** Teclas `W`, `A`, `S`, `D` (o las Flechas de dirección).
* **Cámara / Mirada:** Mover el `Ratón`.
* **Ataque Principal / Interactuar:** Clic Izquierdo del `Ratón`.
* **Esquivar / Dash:** Barra `Espaciadora`.
* **Menú de Pausa:** Tecla `Esc` (Escape).

### 🎮 Configuración: Mando (Gamepad)
* **Movimiento del personaje:** Joystick Izquierdo (`Left Stick`).
* **Cámara / Mirada:** Joystick Derecho (`Right Stick`).
* **Ataque Principal / Interactuar:** Botón `X` (PlayStation) / `A` (Xbox) o Gatillo Derecho (`R2`/`RT`).
* **Esquivar / Dash:** Botón `O` (PlayStation) / `B` (Xbox) o Botón Inferior Izquierdo.
* **Menú de Pausa:** Botón `Options` / `Start`.

---

## ⚙️ Instrucciones de Inicio y Configuración

1. **Descarga el juego:** Descarga el archivo comprimido de la última versión estable (Build).
2. **Extracción:** Descomprime el contenido en una carpeta local (ej. Escritorio).
3. **Ejecución:** Haz doble clic sobre el ejecutable principal `rogueLiteDemo.exe`.

---

## 🔐 Sistema de Autenticación (Menú Principal)

Al arrancar el juego, se presentará la interfaz de acceso para gestionar el progreso de tu personaje en la nube:

* **Registro:** Introduce un correo electrónico válido y una contraseña para crear una nueva partida en Firestore con los parámetros iniciales (100 de salud, 0 monedas).
* **Inicio de Sesión:** Si ya dispones de una cuenta activa, introduce tus credenciales y pulsa **INICIAR SESIÓN** para recuperar de forma automática tu progreso.

---

## ⚠️ MODO SEGURO: Protocolo de Emergencia (Tecla 'L')

> 🔴 **NOTA IMPORTANTE PARA EVALUADORES / DESPLIEGUES EN ENTORNOS RESTRINGIDOS:**
> 
> Debido a que algunos firewalls académicos o restricciones de red locales pueden bloquear el tráfico de los sockets de Firebase o pausar la carga debido a dependencias nativas externas, el juego cuenta con un **Protocolo de Contingencia Automatizado**.
>
> Si la interfaz del menú principal no recibe respuesta de los servidores de Google tras **3 segundos**, se activará visualmente un aviso en la parte inferior de la pantalla.
>
> * **Acción de Bypass:** Pulsa la tecla **`L`** en tu teclado.
> * **Efecto:** El sistema saltará de forma segura el inicio de sesión bloqueado, forzará la activación del flujo del juego y cargará directamente la escena principal (`Scenes/Lobby_3D`) para permitir la evaluación completa de las mecánicas, salas y físicas del juego sin interrupciones de red.

---

## 🛠️ Tecnologías Utilizadas
* **Motor Gráfico:** Unity 6
* **Persistencia de Datos:** Google Firebase Firestore
* **Gestión de Usuarios:** Firebase Authentication
* **Renderizado de Texto:** TextMeshPro (TMP)
---

---

## 👥 Autores y Créditos (Equipo de Desarrollo)

Este proyecto ha sido diseñado, programado y testeado de forma conjunta para el Trabajo de Fin de Grado (TFG) por:

* **Ángel Miguel Felipe ** — Desarrollador de Software / Arquitectura de Inteligencia Artificial (IA), Lógica de Enemigos y Diseño de Sistema de Comportamiento 🗺️🧠
* **Laura Pinel García ** — Desarrolladora de Software / Diseño de Sistemas, Experiencia de Usuario e Interfaz (UI/UX) 💻
* **David Guelar Franch ** — Desarrollador de Software / Mecánicas de Juego Base, Sistema de Combate y Control de Físicas (Gameplay) 🕹️
* **Eugenia de Bertodano Lueña ** — Desarrolladora de Software / Integración de Sistemas Cloud, Autenticación y Persistencia de Datos (Firebase) ☁️

*Agradecimientos especiales al tribunal evaluador por su tiempo y consideración durante la defensa de este proyecto.*
