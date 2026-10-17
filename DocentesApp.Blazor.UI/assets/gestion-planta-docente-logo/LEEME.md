# Logo — Gestión de Planta Docente

Departamento de Materias Básicas · UTN Facultad Regional Rosario.
Misma familia visual que las piezas de las UDB: marino `#001B53`, teal `#00829F`, arco orbital, íconos de línea, tipografía Poppins.
Todos los textos de los SVG están convertidos a trazos, así que no hace falta tener la fuente instalada.

> El escudo de la UTN **no** está incluido ni redibujado. Si va en el encabezado, usar el archivo oficial al lado del logo, separado por una línea vertical fina, como en los banners.

## Qué hay en cada carpeta

| Carpeta | Archivos | Uso |
|---|---|---|
| `svg/` | `logo-completo-light.svg` / `-dark.svg` | Ícono + nombre. *light* = para fondos claros, *dark* = para fondos oscuros |
| | `logo-apilado-light.svg` / `-dark.svg` | Ícono arriba y nombre centrado: login, splash, portadas. Ver `login_preview.png` |
| | `icono-light.svg` / `-dark.svg` | Solo ícono, transparente, para 64 px o más |
| | `icono-chico-light.svg` / `-dark.svg` | Ícono simplificado (sin anillo fino, grilla 2×2) para 24–48 px |
| | `icono-micro.svg` | Versión de 16 px (tile marino lleno) |
| | `icono-app-light.svg` / `-dark.svg` | Ícono dentro de un cuadrado redondeado (tile de app) |
| `png/logo/` | 400, 800, 1600 y 3200 px de ancho | Logo completo transparente |
| `png/logo-apilado/` | 300, 600, 1200 y 2400 px de ancho | Logo apilado transparente |
| `png/icono/` | 16 a 1024 px | Ícono transparente (16 = micro, 32–48 = chico) |
| `favicon/` | `favicon.ico` (16/32/48), `favicon.svg`, `apple-touch-icon.png`, `icon-192.png`, `icon-512.png` | Pestaña del navegador y PWA |
| `css/` | `paleta.css` | Variables de color con tema claro y oscuro |

## Blazor: favicon

Copiar el contenido de `favicon/` a `wwwroot/` y en `App.razor` (o `index.html`) dentro de `<head>`:

```html
<link rel="icon" href="favicon.ico" sizes="any">
<link rel="icon" href="favicon.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="apple-touch-icon.png">
<link rel="stylesheet" href="css/paleta.css">
```

## Logo en el header según el tema

```html
<picture>
  <source srcset="img/logo-completo-dark.svg" media="(prefers-color-scheme: dark)">
  <img src="img/logo-completo-light.svg" alt="Gestión de Planta Docente" height="40">
</picture>
```

## Login (versión apilada)

```html
<picture>
  <source srcset="img/logo-apilado-dark.svg" media="(prefers-color-scheme: dark)">
  <img src="img/logo-apilado-light.svg" alt="Gestión de Planta Docente" width="320">
</picture>
```

Ancho recomendado en la tarjeta de login: 280–360 px.

## Reglas de uso

- Altura mínima del logo completo: 32 px en pantalla. Debajo de eso, usar solo el ícono.
- Dejar aire alrededor equivalente al alto de "GESTIÓN DE".
- No cambiar los colores, deformar, rotar ni agregar sombras.
- El nombre es provisorio: si cambia, se regenera desde `fuente/final.py` (requiere `pip install cairosvg fonttools pillow` y Poppins).
