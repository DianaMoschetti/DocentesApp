# Piezas institucionales — versión clean

Departamento de Materias Básicas · UTN FRRO. Todas en **1920 × 1080 (16:9)**, en SVG (textos convertidos a trazos) y PNG, con variante **clara** y **oscura**.

| Archivo | Pieza |
|---|---|
| `udb-matematica` | U.D.B. Matemática |
| `udb-fisica` | U.D.B. Física |
| `udb-quimica` | U.D.B. Química |
| `udb-cultura-e-idiomas` | U.D.B. Cultura e Idiomas |
| `udb-legislacion-y-economia` | U.D.B. Legislación y Economía |
| `gabinete-sistemas-de-representacion` | Gabinete de Sistemas de Representación |
| `laboratorio-informatico` | Laboratorio Informático y Multidisciplinar |
| `departamento-portada` | Portada del Departamento (con piso y correo) |
| `contacto` | Contacto |

Estructura: `svg/claro`, `svg/oscuro`, `png/claro`, `png/oscuro`. Vista general en `vista-claro.png` y `vista-oscuro.png`.

## Cambios respecto de los originales
- Sin mallas onduladas, grillas de puntos, dígitos binarios ni mapas; tres íconos de línea por área.
- Se mantienen: encabezado institucional, título en teal + marino, barra teal, arco orbital y franja inferior bicolor.
- "INFORMÁTICO" con tilde; se quitó la leyenda "Universalidad Tecnológica Nacional" (tenía un error y repetía el encabezado).

## Escudo UTN
No está redibujado: es el recorte de las imágenes originales (marino para la variante clara, blanco para la oscura), insertado como imagen con `id="escudo-utn"`. Con el archivo oficial vectorial, reemplazar ese elemento o los PNG de `fuente/` y regenerar.

## Regenerar
`python3 fuente/banners.py salida/` (requiere `pip install cairosvg fonttools pillow` y la fuente Poppins). Colores y temas en `THEMES`, textos e íconos en `PIEZAS`.
