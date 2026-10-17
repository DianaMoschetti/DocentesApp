"""Paquete final — Propuesta C (Grilla de designaciones)."""
import math, os, shutil
from textpath import text_path

# ---------------- Paleta ----------------
BRAND = {
    "navy-900": "#001B53",   # azul marino institucional (texto, arco izquierdo)
    "navy-800": "#0A2766",
    "navy-700": "#1E3A78",
    "teal-600": "#00829F",   # teal institucional (acentos, arco derecho)
    "teal-400": "#2BB3CC",   # teal para fondos oscuros
    "teal-100": "#CDE8EE",   # tinte para celdas / fondos suaves
    "gray-50":  "#F4F6F8",   # fondo claro (como los banners)
    "gray-500": "#3A4A6B",   # texto secundario
    "gray-300": "#B8C6DD",   # texto secundario sobre oscuro
    "white":    "#FFFFFF",
}
PAL = {
    "light": dict(navy=BRAND["navy-900"], teal=BRAND["teal-600"], tint=BRAND["teal-100"],
                  ink2=BRAND["gray-500"], badge_ink=BRAND["white"], tile=BRAND["gray-50"]),
    "dark":  dict(navy=BRAND["white"], teal=BRAND["teal-400"], tint="#1E4F86",
                  ink2=BRAND["gray-300"], badge_ink=BRAND["navy-900"], tile=BRAND["navy-900"]),
}

def arc(cx, cy, r, a0, a1):
    p = lambda a: (cx + r*math.cos(math.radians(a)), cy + r*math.sin(math.radians(a)))
    (x0, y0), (x1, y1) = p(a0), p(a1)
    large = 1 if (a1 - a0) % 360 > 180 else 0
    return f"M{x0:.2f} {y0:.2f} A{r} {r} 0 {large} 1 {x1:.2f} {y1:.2f}"

def person(cx, cy, s, fill):
    return (f'<circle cx="{cx}" cy="{cy-4.2*s:.2f}" r="{3.6*s:.2f}" fill="{fill}"/>'
            f'<path d="M{cx-6.6*s:.2f} {cy+7*s:.2f} Q{cx-6.6*s:.2f} {cy+0.6*s:.2f} {cx} {cy+0.6*s:.2f} '
            f'Q{cx+6.6*s:.2f} {cy+0.6*s:.2f} {cx+6.6*s:.2f} {cy+7*s:.2f} Z" fill="{fill}"/>')

def icon_body(c, uid, small=False):
    """Contenido del ícono en un viewBox 0 0 120 120. El hueco alrededor del docente
    es una máscara real (transparente), así funciona sobre cualquier fondo."""
    if small:   # simplificado para ≤ 48 px: sin anillo fino ni puntos, grilla 2x2, trazos más gruesos
        frame = dict(x=26, y=30, w=60, h=54, rx=7, sw=4.5)
        grid = dict(x0=34, y0=38, w=19, h=15, g=6, n=2)
        fills = [["navy", "teal"], ["tint", "navy"]]
        badge = dict(cx=84, cy=82, r=19, gap=5, ps=1.3)
        orbit = (f'<path d="{arc(60,60,51,112,248)}" fill="none" stroke="{c["navy"]}" stroke-width="10" stroke-linecap="round"/>'
                 f'<path d="{arc(60,60,51,-60,40)}" fill="none" stroke="{c["teal"]}" stroke-width="8" stroke-linecap="round"/>')
    else:
        frame = dict(x=28, y=31, w=56, h=51, rx=5, sw=2.6)
        grid = dict(x0=33, y0=38, w=13, h=10, g=3, n=3)
        fills = [["navy", "teal", "tint"], ["tint", "navy", "teal"], ["teal", "tint", None]]
        badge = dict(cx=81, cy=80, r=14, gap=3, ps=0.95)
        p1 = (60+54*math.cos(math.radians(212)), 60+54*math.sin(math.radians(212)))
        p2 = (60+54*math.cos(math.radians(236)), 60+54*math.sin(math.radians(236)))
        orbit = (f'<circle cx="60" cy="60" r="54" fill="none" stroke="{c["teal"]}" stroke-width="1.6" opacity=".55"/>'
                 f'<path d="{arc(60,60,47,110,250)}" fill="none" stroke="{c["navy"]}" stroke-width="7" stroke-linecap="round"/>'
                 f'<path d="{arc(60,60,49,-62,58)}" fill="none" stroke="{c["teal"]}" stroke-width="5" stroke-linecap="round"/>'
                 f'<circle cx="{p1[0]:.2f}" cy="{p1[1]:.2f}" r="4.2" fill="{c["teal"]}"/>'
                 f'<circle cx="{p2[0]:.2f}" cy="{p2[1]:.2f}" r="2.6" fill="{c["navy"]}"/>')
    cells = []
    for r in range(grid["n"]):
        for k in range(grid["n"]):
            key = fills[r][k]
            x = grid["x0"] + k*(grid["w"]+grid["g"]); y = grid["y0"] + r*(grid["h"]+grid["g"])
            f = c[key] if key else "none"
            st = c["teal"] if key in ("tint", None) else c[key]
            cells.append(f'<rect x="{x}" y="{y}" width="{grid["w"]}" height="{grid["h"]}" rx="1.8" fill="{f}" stroke="{st}" stroke-width="1.6"/>')
    b = badge
    return f'''<defs><mask id="{uid}" maskUnits="userSpaceOnUse" x="0" y="0" width="120" height="120">
  <rect width="120" height="120" fill="#fff"/><circle cx="{b['cx']}" cy="{b['cy']}" r="{b['r']+b['gap']}" fill="#000"/></mask></defs>
<g mask="url(#{uid})">{orbit}
  <rect x="{frame['x']}" y="{frame['y']}" width="{frame['w']}" height="{frame['h']}" rx="{frame['rx']}" fill="none" stroke="{c['navy']}" stroke-width="{frame['sw']}"/>
  {''.join(cells)}</g>
<circle cx="{b['cx']}" cy="{b['cy']}" r="{b['r']}" fill="{c['navy']}"/>
{person(b['cx'], b['cy']+1, b['ps'], c['badge_ink'])}'''

def icon_svg(mode, small=False, tile=False):
    c = PAL[mode]; uid = f"pd-{mode}-{'s' if small else 'f'}{'-t' if tile else ''}"
    back = f'<rect width="120" height="120" rx="26" fill="{c["tile"]}"/>' if tile else ""
    inner = icon_body(c, uid, small)
    if tile:  # un poco de aire dentro del cuadrado
        inner = f'<g transform="translate(9 9) scale(.85)">{inner}</g>'
    return (f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="120" height="120">'
            f'<title>Gestión de Planta Docente</title>{back}{inner}</svg>\n')

def full_svg(mode):
    c = PAL[mode]; tx = 140
    d1, w1 = text_path("GESTIÓN DE", tx, 34, 15, "Medium", 0.16)
    d2, w2 = text_path("PLANTA DOCENTE", tx, 70, 34, "Bold", 0.01)
    d3, w3 = text_path("DEPARTAMENTO DE MATERIAS BÁSICAS · UTN FRRO", tx, 108, 11.5, "Medium", 0.08)
    W = round(tx + max(w1, w2, w3) + 4)
    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {W} 124" width="{W}" height="124">
<title>Gestión de Planta Docente — Departamento de Materias Básicas, UTN FRRO</title>
<g transform="translate(2 2)">{icon_body(c, f"pd-{mode}-full")}</g>
<path d="{d1}" fill="{c['teal']}"/>
<path d="{d2}" fill="{c['navy']}"/>
<rect x="{tx}" y="82" width="46" height="4" rx="2" fill="{c['teal']}"/>
<path d="{d3}" fill="{c['ink2']}"/>
</svg>
''', W


def stacked_svg(mode):
    """Versión apilada (ícono arriba, nombre centrado abajo) para login / portadas."""
    c = PAL[mode]
    lines = [("GESTIÓN DE", 15, "Medium", 0.16, c["teal"], 0),
             ("PLANTA DOCENTE", 34, "Bold", 0.01, c["navy"], 0),
             ("DEPARTAMENTO DE MATERIAS BÁSICAS · UTN FRRO", 11.5, "Medium", 0.08, c["ink2"], 0)]
    widths = [text_path(t, 0, 0, s, w, tr)[1] for t, s, w, tr, _, _ in lines]
    W = round(max(widths) + 8); cx = W / 2
    ys = [168, 206, 246]
    icon = f'<g transform="translate({cx-60:.2f} 0)">{icon_body(c, f"pd-{mode}-stack")}</g>'
    paths = "".join(f'<path d="{text_path(t, cx-w/2, y, s, wt, tr)[0]}" fill="{col}"/>'
                    for (t, s, wt, tr, col, _), w, y in zip(lines, widths, ys))
    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {W} 256" width="{W}" height="256">
<title>Gestión de Planta Docente — Departamento de Materias Básicas, UTN FRRO</title>
{icon}{paths}
<rect x="{cx-23:.2f}" y="218" width="46" height="4" rx="2" fill="{c['teal']}"/>
</svg>
'''

def micro_svg():
    """Para 16 px: sin arcos, tile marino lleno, grilla 2x2 y docente. Solo favicon-16."""
    n, t, w = BRAND["navy-900"], BRAND["teal-400"], BRAND["white"]
    return (f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" width="16" height="16">'
            f'<title>Gestión de Planta Docente</title>'
            f'<rect width="16" height="16" rx="3.5" fill="{n}"/>'
            f'<rect x="2" y="2" width="5" height="4" rx=".8" fill="{w}"/>'
            f'<rect x="8.5" y="2" width="5" height="4" rx=".8" fill="{t}"/>'
            f'<rect x="2" y="7.5" width="5" height="4" rx=".8" fill="{t}"/>'
            f'<circle cx="11.6" cy="11.6" r="4" fill="{t}" stroke="{n}" stroke-width="1.2"/>'
            f'<circle cx="11.6" cy="10.5" r="1.25" fill="{n}"/>'
            f'<path d="M9.4 14.2 Q9.4 12.2 11.6 12.2 Q13.8 12.2 13.8 14.2 Z" fill="{n}"/></svg>\n')

if __name__ == "__main__":
    import cairosvg
    from PIL import Image
    OUT = "gestion-planta-docente-logo"
    shutil.rmtree(OUT, ignore_errors=True)
    for d in ["svg", "png/logo", "png/icono", "favicon", "css"]:
        os.makedirs(f"{OUT}/{d}", exist_ok=True)

    # SVG
    for m in PAL:
        s, W = full_svg(m); open(f"{OUT}/svg/logo-completo-{m}.svg", "w").write(s)
        open(f"{OUT}/svg/icono-{m}.svg", "w").write(icon_svg(m))
        open(f"{OUT}/svg/icono-chico-{m}.svg", "w").write(icon_svg(m, small=True))
        open(f"{OUT}/svg/icono-app-{m}.svg", "w").write(icon_svg(m, tile=True))
        open(f"{OUT}/svg/logo-apilado-{m}.svg", "w").write(stacked_svg(m))

    # PNG logo completo (transparente) en varios anchos
    for m in PAL:
        for w in (400, 800, 1600, 3200):
            cairosvg.svg2png(url=f"{OUT}/svg/logo-completo-{m}.svg", write_to=f"{OUT}/png/logo/logo-completo-{m}-{w}.png", output_width=w)
    os.makedirs(f"{OUT}/png/logo-apilado", exist_ok=True)
    for m in PAL:
        for w in (300, 600, 1200, 2400):
            cairosvg.svg2png(url=f"{OUT}/svg/logo-apilado-{m}.svg", write_to=f"{OUT}/png/logo-apilado/logo-apilado-{m}-{w}.png", output_width=w)
    # PNG ícono: chico (≤48) usa la versión simplificada
    for m in PAL:
        for s in (16, 32, 48, 64, 128, 256, 512, 1024):
            if s == 16:
                open(f"{OUT}/svg/icono-micro.svg", "w").write(micro_svg())
                cairosvg.svg2png(url=f"{OUT}/svg/icono-micro.svg", write_to=f"{OUT}/png/icono/icono-{m}-16.png", output_width=16)
                continue
            src = "icono-chico" if s <= 48 else "icono"
            cairosvg.svg2png(url=f"{OUT}/svg/{src}-{m}.svg", write_to=f"{OUT}/png/icono/icono-{m}-{s}.png", output_width=s)

    # Favicon y app icons (con fondo, para que se vea en pestañas claras y oscuras)
    F = f"{OUT}/favicon"
    open(f"{F}/favicon.svg", "w").write(icon_svg("light", small=True, tile=True))
    open(f"{F}/favicon-16.svg", "w").write(micro_svg())
    cairosvg.svg2png(url=f"{F}/favicon-16.svg", write_to=f"{F}/favicon-16.png", output_width=16)
    for s in (32, 48):
        cairosvg.svg2png(url=f"{F}/favicon.svg", write_to=f"{F}/favicon-{s}.png", output_width=s)
    imgs = [Image.open(f"{F}/favicon-{s}.png") for s in (16, 32, 48)]
    imgs[2].save(f"{F}/favicon.ico", sizes=[(16, 16), (32, 32), (48, 48)], append_images=imgs[:2])
    cairosvg.svg2png(url=f"{OUT}/svg/icono-app-light.svg", write_to=f"{F}/apple-touch-icon.png", output_width=180)
    for s in (192, 512):
        cairosvg.svg2png(url=f"{OUT}/svg/icono-app-light.svg", write_to=f"{F}/icon-{s}.png", output_width=s)
    print("ok")
