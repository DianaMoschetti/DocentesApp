"""Piezas institucionales del Departamento de Materias Básicas — versión clean.
Genera SVG (textos convertidos a trazos) + PNG al tamaño original."""
import base64, math, os, sys
HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
from textpath import text_path

_b64 = lambda f: base64.b64encode(open(os.path.join(HERE, f), "rb").read()).decode()
THEMES = {
    "claro":  dict(NAVY="#001B53", TEAL="#00829F", TINT="#CDE8EE", BG="#F4F6F8", INK2="#3A4A6B",
                   CIRCLE="#FFFFFF", FOOT1="#001B53", BAND="#001B53", BADGE_INK="#FFFFFF",
                   ENV_INK="#FFFFFF", DOT_INK="#FFFFFF", ESC=_b64("escudo_navy.png")),
    "oscuro": dict(NAVY="#FFFFFF", TEAL="#2BB3CC", TINT="#1E4F86", BG="#001B53", INK2="#B8C6DD",
                   CIRCLE="#0A2766", FOOT1="#FFFFFF", BAND="#0A2766", BADGE_INK="#001B53",
                   ENV_INK="#001B53", DOT_INK="#FFFFFF", ESC=_b64("escudo_white.png")),
}
def set_theme(name):
    globals().update(THEMES[name])
set_theme("claro")
ESC_W, ESC_H = 113, 141

def arc(cx, cy, r, a0, a1):
    p = lambda a: (cx + r*math.cos(math.radians(a)), cy + r*math.sin(math.radians(a)))
    (x0, y0), (x1, y1) = p(a0), p(a1)
    return f"M{x0:.1f} {y0:.1f} A{r:.1f} {r:.1f} 0 {1 if (a1-a0)%360>180 else 0} 1 {x1:.1f} {y1:.1f}"

def txt(s, x, y, size, weight, fill, tr=0.0, anchor="start"):
    if anchor != "start":
        w = text_path(s, 0, 0, size, weight, tr)[1]
        x = x - (w if anchor == "end" else w/2)
    d, w = text_path(s, x, y, size, weight, tr)
    return f'<path d="{d}" fill="{fill}"/>', w

def tw(s, size, weight, tr=0.0):
    return text_path(s, 0, 0, size, weight, tr)[1]

# ---------------- Íconos (grilla 100x100, trazo de línea) ----------------
S = 'stroke-linecap="round" stroke-linejoin="round"'
def ic_wave(u):  return f'''<path d="M14 84 H90 M20 90 V12" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M20 52 C30 22 40 22 48 52 S66 82 76 52 S88 30 92 36" stroke="{TEAL}" stroke-width="4" {S}/>'''
def ic_venn(u):  return f'''<defs><clipPath id="v{u}"><circle cx="38" cy="50" r="26"/></clipPath></defs>
<circle cx="62" cy="50" r="26" fill="{TEAL}" clip-path="url(#v{u})"/>
<circle cx="38" cy="50" r="26" stroke="{NAVY}" stroke-width="3.5" {S}/><circle cx="62" cy="50" r="26" stroke="{NAVY}" stroke-width="3.5" {S}/>'''
def ic_pyr(u):   return f'''<path d="M18 76 L66 88 L86 70 L40 64 Z" fill="{TINT}" stroke="none"/>
<path d="M50 12 L18 76 L66 88 L86 70 Z M50 12 L66 88" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M50 12 L40 64 M18 76 L40 64 L86 70" stroke="{NAVY}" stroke-width="2" stroke-dasharray="4 5" {S}/>
<path d="M50 12 V72" stroke="{TEAL}" stroke-width="3" {S}/>'''
def ic_atom(u):  return "".join(f'<ellipse cx="50" cy="50" rx="40" ry="14" transform="rotate({a} 50 50)" stroke="{NAVY}" stroke-width="3.2" {S}/>' for a in (0, 60, 120)) + f'<circle cx="50" cy="50" r="7" fill="{TEAL}"/>'
def ic_pend(u):  return f'''<path d="M22 12 H78" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M50 12 L30 72" stroke="{NAVY}" stroke-width="3" {S}/><path d="M50 12 V78" stroke="{NAVY}" stroke-width="2" stroke-dasharray="4 5" {S}/>
<path d="M28 80 Q50 94 72 80" stroke="{TEAL}" stroke-width="2.5" stroke-dasharray="4 5" {S}/>
<circle cx="30" cy="74" r="9" fill="{TEAL}"/>'''
def ic_lens(u):  return f'''<path d="M50 14 Q64 50 50 86 Q36 50 50 14 Z" fill="{TINT}" stroke="{TEAL}" stroke-width="3.2" {S}/>
<path d="M8 32 H50 L88 50 M8 50 H88 M8 68 H50 L88 50" stroke="{NAVY}" stroke-width="3" {S}/>'''
def ic_globe(u): return f'''<circle cx="50" cy="50" r="36" stroke="{NAVY}" stroke-width="3.5" {S}/>
<ellipse cx="50" cy="50" rx="15" ry="36" stroke="{TEAL}" stroke-width="3" {S}/>
<path d="M14 50 H86 M20 32 H80 M20 68 H80" stroke="{NAVY}" stroke-width="2.5" {S}/>'''
def ic_book(u):  return f'''<path d="M50 30 C40 22 24 22 12 26 V78 C24 74 40 74 50 82 Z" fill="{TINT}" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M50 30 C60 22 76 22 88 26 V78 C76 74 60 74 50 82 Z" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M60 40 H78 M60 50 H78 M60 60 H72" stroke="{TEAL}" stroke-width="3" {S}/>'''
def ic_talk(u):  return f'''<path d="M12 22 h44 a6 6 0 0 1 6 6 v22 a6 6 0 0 1 -6 6 h-30 l-10 10 v-10 a6 6 0 0 1 -4 -6 v-22 a6 6 0 0 1 6 -6 z" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M44 44 h40 a6 6 0 0 1 6 6 v20 a6 6 0 0 1 -6 6 h-4 v10 l-10 -10 h-26 a6 6 0 0 1 -6 -6 v-20 a6 6 0 0 1 6 -6 z" fill="{TEAL}" stroke="{TEAL}" stroke-width="3.2" {S}/>
<circle cx="54" cy="60" r="3" fill="{DOT_INK}"/><circle cx="64" cy="60" r="3" fill="{DOT_INK}"/><circle cx="74" cy="60" r="3" fill="{DOT_INK}"/>'''
def ic_doc(u):   return f'''<path d="M22 10 H60 L76 26 V88 H22 Z M60 10 V26 H76" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M32 40 H62 M32 52 H62 M32 64 H48" stroke="{NAVY}" stroke-width="3" {S}/>
<circle cx="72" cy="76" r="15" fill="{TEAL}"/><path d="M65 76 l5 5 l9 -10" stroke="{DOT_INK}" stroke-width="3.5" {S}/>'''
def ic_bars(u):  return f'''<path d="M10 88 H92" stroke="{NAVY}" stroke-width="3.5" {S}/>
''' + "".join(f'<rect x="{x}" y="{y}" width="12" height="{86-y}" rx="2" fill="{TINT}" stroke="{TEAL}" stroke-width="2.8"/>' for x, y in ((16, 62), (36, 50), (56, 40), (76, 26))) + f'''
<path d="M14 54 L38 38 L56 44 L86 12 M74 12 H86 V24" stroke="{NAVY}" stroke-width="3.5" {S}/>'''
def ic_supply(u):return f'''<path d="M14 10 V88 H92" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M22 20 C40 56 60 72 88 78" stroke="{TEAL}" stroke-width="3.5" {S}/>
<path d="M22 80 C46 70 70 44 84 18" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M14 61 H55 V88" stroke="{NAVY}" stroke-width="2" stroke-dasharray="4 5" {S}/><circle cx="55" cy="61" r="4.5" fill="{NAVY}"/>'''
def ic_laptop(u):return f'''<rect x="18" y="20" width="64" height="44" rx="4" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M8 76 H92 L86 82 H14 Z" fill="{NAVY}" stroke="{NAVY}" stroke-width="3" {S}/>
<path d="M40 34 L32 42 L40 50 M60 34 L68 42 L60 50 M54 32 L46 52" stroke="{TEAL}" stroke-width="3.5" {S}/>'''
def ic_flow(u):  return f'''<rect x="34" y="8" width="32" height="14" rx="7" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M50 22 V32 M50 64 V72 H24 V78 M50 72 H76 V78" stroke="{NAVY}" stroke-width="3" {S}/>
<path d="M50 32 L66 48 L50 64 L34 48 Z" fill="{TEAL}" stroke="{TEAL}" stroke-width="3" {S}/>
<rect x="10" y="78" width="28" height="14" rx="3" stroke="{NAVY}" stroke-width="3.2" {S}/><rect x="62" y="78" width="28" height="14" rx="3" stroke="{NAVY}" stroke-width="3.2" {S}/>'''
def ic_db(u):    return f'''<ellipse cx="50" cy="20" rx="30" ry="10" fill="{TINT}" stroke="{NAVY}" stroke-width="3.2" {S}/>
<path d="M20 20 V80 C20 94 80 94 80 80 V20 M20 40 C20 54 80 54 80 40 M20 60 C20 74 80 74 80 60" stroke="{NAVY}" stroke-width="3.2" {S}/>
<circle cx="68" cy="52" r="3" fill="{TEAL}"/><circle cx="68" cy="72" r="3" fill="{TEAL}"/>'''

def ic_flask(u): return f'''<path d="M29 62 H71 L81 82 Q83 90 75 90 H25 Q17 90 19 82 Z" fill="{TEAL}"/>
<path d="M36 10 H64 M42 10 V38 L18 82 Q15 90 24 90 H76 Q85 90 82 82 L58 38 V10" stroke="{NAVY}" stroke-width="3.5" {S}/>
<circle cx="44" cy="76" r="4" fill="{DOT_INK}"/><circle cx="56" cy="70" r="3" fill="{DOT_INK}"/>
<circle cx="50" cy="48" r="3" stroke="{TEAL}" stroke-width="2.5"/><circle cx="56" cy="30" r="2.5" stroke="{TEAL}" stroke-width="2.2"/>'''
def ic_benz(u):
    pts = [(50 + 32*math.cos(math.radians(a)), 50 + 32*math.sin(math.radians(a))) for a in range(-90, 270, 60)]
    poly = " ".join(f"{x:.1f},{y:.1f}" for x, y in pts)
    nodes = "".join(f'<circle cx="{x:.1f}" cy="{y:.1f}" r="{6 if i%2==0 else 4.5}" fill="{TEAL if i%2==0 else NAVY}"/>' for i, (x, y) in enumerate(pts))
    return f'<polygon points="{poly}" stroke="{NAVY}" stroke-width="3.5" {S}/><circle cx="50" cy="50" r="15" stroke="{TEAL}" stroke-width="3.2"/>{nodes}'
def ic_tubes(u): return f'''<path d="M30 52 V74 A8 8 0 0 0 46 74 V52 Z" fill="{TEAL}"/>
<path d="M56 62 V74 A8 8 0 0 0 72 74 V62 Z" fill="{TINT}"/>
<path d="M30 12 V74 A8 8 0 0 0 46 74 V12 M56 12 V74 A8 8 0 0 0 72 74 V12 M26 12 H50 M52 12 H76" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M14 38 H86 M20 38 V92 M80 38 V92" stroke="{NAVY}" stroke-width="3" {S}/>'''
def ic_iso(u): return f'''<path d="M50 14 L82 32 L50 50 L18 32 Z" fill="{TINT}"/><path d="M50 50 L82 32 V68 L50 86 Z" fill="{TEAL}"/>
<path d="M50 14 L82 32 V68 L50 86 L18 68 V32 Z M18 32 L50 50 L82 32 M50 50 V86" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M18 68 L50 50" stroke="{NAVY}" stroke-width="2" stroke-dasharray="4 5" {S}/>'''
def ic_compass(u): return f'''<path d="M24 84 Q50 98 76 84" stroke="{TEAL}" stroke-width="2.5" stroke-dasharray="4 5" {S}/>
<circle cx="50" cy="16" r="7" stroke="{NAVY}" stroke-width="3.5"/><path d="M50 4 V9" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M46 22 L26 82 M54 22 L72 74" stroke="{NAVY}" stroke-width="4" {S}/>
<path d="M72 74 L76 86 L68 78 Z" fill="{TEAL}" stroke="{TEAL}" stroke-width="2.5" {S}/><path d="M34 58 Q50 64 66 58" stroke="{NAVY}" stroke-width="2.5" {S}/>'''
def ic_square(u): return f'''<path d="M28 74 V48 L54 74 Z" fill="{TINT}"/>
<path d="M14 88 V18 L84 88 Z M28 74 V48 L54 74 Z" stroke="{NAVY}" stroke-width="3.5" {S}/>
<path d="M14 76 H26 V88" stroke="{TEAL}" stroke-width="3" {S}/>
<path d="M60 20 L88 48" stroke="{TEAL}" stroke-width="4" {S}/><path d="M66 26 l-4 4 M74 34 l-4 4 M82 42 l-4 4" stroke="{TEAL}" stroke-width="2.5" {S}/>'''

# Íconos blancos para los círculos de la portada
def bd_graph(): return f'<path d="M18 80 H84 M24 86 V16" stroke="{BADGE_INK}" stroke-width="5" {S}/><path d="M24 70 C40 70 44 30 58 30 S70 56 82 40" stroke="{BADGE_INK}" stroke-width="5" {S}/>'
def bd_int():   return f'<path d="M58 14 C48 10 44 20 46 34 L50 66 C52 80 46 90 36 86" stroke="{BADGE_INK}" stroke-width="6" {S}/><path d="M60 58 L76 74 M76 58 L60 74" stroke="{BADGE_INK}" stroke-width="5" {S}/>'
def bd_atom():  return "".join(f'<ellipse cx="50" cy="50" rx="36" ry="12" transform="rotate({a} 50 50)" stroke="{BADGE_INK}" stroke-width="4.5" {S}/>' for a in (0, 60, 120)) + f'<circle cx="50" cy="50" r="7" fill="{BADGE_INK}"/>'
def bd_cube():  return f'<path d="M22 34 L50 20 L78 34 V66 L50 80 L22 66 Z M22 34 L50 48 L78 34 M50 48 V80" stroke="{BADGE_INK}" stroke-width="5" {S}/>'

def place(fn, x, y, size, uid):
    return f'<g fill="none" transform="translate({x:.1f} {y:.1f}) scale({size/100:.4f})">{fn(uid)}</g>'

# ---------------- Bloques comunes ----------------
def orbit(cx, cy, r, inner=""):
    p1 = (cx + r*1.12*math.cos(math.radians(214)), cy + r*1.12*math.sin(math.radians(214)))
    p2 = (cx + r*1.12*math.cos(math.radians(236)), cy + r*1.12*math.sin(math.radians(236)))
    return f'''<g id="orbita">
  <circle cx="{cx:.1f}" cy="{cy:.1f}" r="{r*1.12:.1f}" fill="none" stroke="{TEAL}" stroke-width="{r*0.008:.1f}" opacity=".45"/>
  <circle cx="{cx:.1f}" cy="{cy:.1f}" r="{r:.1f}" fill="{CIRCLE}"/>
  <path d="{arc(cx, cy, r, 108, 252)}" fill="none" stroke="{NAVY}" stroke-width="{r*0.07:.1f}" stroke-linecap="round"/>
  <path d="{arc(cx, cy, r*1.02, -64, 56)}" fill="none" stroke="{TEAL}" stroke-width="{r*0.05:.1f}" stroke-linecap="round"/>
  <circle cx="{p1[0]:.1f}" cy="{p1[1]:.1f}" r="{r*0.045:.1f}" fill="{TEAL}"/>
  <circle cx="{p2[0]:.1f}" cy="{p2[1]:.1f}" r="{r*0.026:.1f}" fill="{NAVY}"/>
</g>{inner}'''

def header(x, y, h):
    """Escudo (imagen recortada, reemplazable) + UTN + Facultad Regional Rosario."""
    ew = h * ESC_W / ESC_H
    out = [f'<image id="escudo-utn" x="{x:.1f}" y="{y:.1f}" width="{ew:.1f}" height="{h:.1f}" href="data:image/png;base64,{ESC}"/>']
    x1 = x + ew + h*0.32
    out.append(f'<rect x="{x1:.1f}" y="{y+h*0.08:.1f}" width="{max(1.5,h*0.018):.1f}" height="{h*0.84:.1f}" fill="{NAVY}" opacity=".8"/>')
    x2 = x1 + h*0.32
    p, w = txt("UTN", x2, y + h*0.86, h*1.02, "Bold", NAVY, 0.0)
    out.append(p)
    x3 = x2 + w + h*0.30
    out.append(f'<rect x="{x3:.1f}" y="{y+h*0.14:.1f}" width="{max(1.2,h*0.012):.1f}" height="{h*0.72:.1f}" fill="{NAVY}" opacity=".6"/>')
    x4 = x3 + h*0.24
    out.append(txt("FACULTAD REGIONAL", x4, y + h*0.44, h*0.25, "Medium", NAVY, 0.04)[0])
    out.append(txt("ROSARIO", x4, y + h*0.80, h*0.25, "Medium", NAVY, 0.04)[0])
    return "\n".join(out)

def footer_bar(W, H, split=0.36, h=None):
    h = h or H*0.022
    return (f'<rect x="0" y="{H-h:.1f}" width="{W*split:.1f}" height="{h:.1f}" fill="{FOOT1}"/>'
            f'<rect x="{W*split:.1f}" y="{H-h:.1f}" width="{W*(1-split):.1f}" height="{h:.1f}" fill="{TEAL}"/>')

def title_block(lines, x, y_center, max_w, base):
    """lines: [(texto, rel_size, color, weight, tracking)]. Escala para que entre en max_w."""
    widest = max(tw(t, base*rs, wt, tr) for t, rs, _, wt, tr in lines)
    k = min(1.0, max_w / widest)
    sizes = [base*rs*k for _, rs, *_ in lines]
    lh = [s*1.08 for s in sizes]
    total = sum(lh) + base*k*0.38
    y = y_center - total/2
    out = []
    for (t, rs, col, wt, tr), s, l in zip(lines, sizes, lh):
        y += l*0.86
        out.append(txt(t, x, y, s, wt, col, tr)[0])
        y += l*0.14
    y += base*k*0.22
    out.append(f'<rect x="{x:.1f}" y="{y:.1f}" width="{base*k*1.55:.1f}" height="{base*k*0.11:.1f}" rx="{base*k*0.05:.1f}" fill="{TEAL}"/>')
    return "\n".join(out)

def wrap(W, H, body, title):
    return f'''<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 {W} {H}" width="{W}" height="{H}">
<title>{title}</title>
<rect width="{W}" height="{H}" fill="{BG}"/>
{body}
</svg>
'''

def udb_banner(W, H, lines, icons, title):
    m = W*0.055
    hh = H*0.115
    r = min(H*0.33, W*0.2)
    cx, cy = W - m - r*1.12, H*0.5
    isz = r*0.62
    pos = [(cx - isz/2, cy - r*0.80), (cx - r*0.76, cy + r*0.02), (cx + r*0.76 - isz, cy + r*0.02)]
    inner = "".join(place(fn, x, y, isz, f"{title[:3]}{i}") for i, (fn, (x, y)) in enumerate(zip(icons, pos)))
    tx = m
    max_w = cx - r*1.25 - tx
    body = "\n".join([header(m, H*0.1, hh),
                      title_block(lines, tx, H*0.6, max_w, H*0.12),
                      orbit(cx, cy, r, inner),
                      footer_bar(W, H)])
    return wrap(W, H, body, title)

def udb(name1, name2=None):
    L = [("U.D.B.", 1.0, TEAL, "Bold", 0.02), (name1, 1.0, NAVY, "Bold", 0.0)]
    if name2: L.append((name2, 1.0, NAVY, "Bold", 0.0))
    return L

# ---------------- Piezas ----------------
def portada(W=1920, H=1080):
    m = W*0.06; band = H*0.085
    r = H*0.36; cx, cy = W - m - r*0.95, H*0.44
    b = r*0.46; gap = r*0.10
    bx0 = cx - (2*b + gap)/2; by0 = cy - (2*b + gap)/2
    badges = ""
    for i, (fn, col) in enumerate([(bd_graph, NAVY), (bd_int, TEAL), (bd_atom, TEAL), (bd_cube, NAVY)]):  # en oscuro NAVY=blanco
        x = bx0 + (i % 2)*(b+gap); y = by0 + (i//2)*(b+gap)
        badges += f'<circle cx="{x+b/2:.1f}" cy="{y+b/2:.1f}" r="{b/2:.1f}" fill="{col}"/>' + f'<g fill="none" transform="translate({x+b*0.2:.1f} {y+b*0.2:.1f}) scale({b*0.6/100:.4f})">{fn()}</g>'
    lines = [("DEPARTAMENTO DE", 0.42, TEAL, "Bold", 0.03), ("MATERIAS", 1.0, NAVY, "Bold", 0.0), ("BÁSICAS", 1.0, NAVY, "Bold", 0.0)]
    # pie con ubicación y correo
    fy = H - band
    w_piso = tw("4° PISO", band*0.27, "Medium", 0.04); w_mail = tw("BASICAS@FRRO.UTN.EDU.AR", band*0.27, "Medium", 0.04)
    total = band*0.68 + w_piso + band*1.1 + band*0.68 + w_mail
    pin_x = (W - total)/2
    pin = (f'<g transform="translate({pin_x:.1f} {fy+band*0.24:.1f}) scale({band*0.52/100:.4f})">'
           f'<path d="M50 94 C28 66 18 52 18 36 A32 32 0 0 1 82 36 C82 52 72 66 50 94 Z" fill="none" stroke="{TEAL}" stroke-width="8" stroke-linejoin="round"/>'
           f'<circle cx="50" cy="36" r="11" fill="none" stroke="{TEAL}" stroke-width="8"/></g>')
    t1, w1 = txt("4° PISO", pin_x + band*0.68, fy + band*0.62, band*0.27, "Medium", "#fff", 0.04)
    sep_x = pin_x + band*0.68 + w1 + band*0.55
    mail_x = sep_x + band*0.55
    env = (f'<g transform="translate({mail_x:.1f} {fy+band*0.27:.1f}) scale({band*0.46/100:.4f})">'
           f'<rect x="6" y="16" width="88" height="68" rx="8" fill="none" stroke="{TEAL}" stroke-width="8"/>'
           f'<path d="M10 22 L50 54 L90 22" fill="none" stroke="{TEAL}" stroke-width="8" stroke-linejoin="round" stroke-linecap="round"/></g>')
    t2, _ = txt("BASICAS@FRRO.UTN.EDU.AR", mail_x + band*0.68, fy + band*0.62, band*0.27, "Medium", "#fff", 0.04)
    body = "\n".join([header(m, H*0.075, H*0.1),
                      title_block(lines, m, H*0.5, cx - r*1.2 - m, H*0.15),
                      orbit(cx, cy, r, badges),
                      f'<rect x="0" y="{fy:.1f}" width="{W}" height="{band:.1f}" fill="{BAND}"/>',
                      pin, t1, f'<rect x="{sep_x:.1f}" y="{fy+band*0.25:.1f}" width="2" height="{band*0.5:.1f}" fill="#fff" opacity=".5"/>', env, t2])
    return wrap(W, H, body, "Departamento de Materias Básicas — UTN FRRO")

def contacto(W=1920, H=1080):
    cy = H*0.47
    r_env = H*0.1
    email = "basicas@frro.utn.edu.ar"
    size = H*0.105
    w = tw(email, size, "SemiBold" if False else "Bold", 0.0)
    total = r_env*2 + H*0.06 + w
    if total > W*0.86:
        size *= (W*0.86 - r_env*2 - H*0.06)/w; w = tw(email, size, "Bold")
        total = r_env*2 + H*0.06 + w
    x0 = (W - total)/2
    ex, ey = x0 + r_env, cy
    env = (f'<circle cx="{ex:.1f}" cy="{ey:.1f}" r="{r_env:.1f}" fill="{NAVY}"/>'
           f'<g transform="translate({ex-r_env*0.52:.1f} {ey-r_env*0.52:.1f}) scale({r_env*1.04/100:.4f})">'
           f'<rect x="8" y="18" width="84" height="64" rx="8" fill="none" stroke="{ENV_INK}" stroke-width="7"/>'
           f'<path d="M12 24 L50 54 L88 24" fill="none" stroke="{ENV_INK}" stroke-width="7" stroke-linejoin="round" stroke-linecap="round"/></g>')
    t, _ = txt(email, x0 + r_env*2 + H*0.06, cy + size*0.36, size, "Bold", TEAL)
    # arco orbital decorativo, arriba a la derecha (como el original)
    R = H*0.62; ocx, ocy = W*0.98, H*0.70
    deco = (f'<path d="{arc(ocx, ocy, R, 226, 274)}" fill="none" stroke="{TEAL}" stroke-width="3" opacity=".7"/>'
            f'<circle cx="{ocx+R*math.cos(math.radians(238)):.1f}" cy="{ocy+R*math.sin(math.radians(238)):.1f}" r="{H*0.02:.1f}" fill="{TEAL}"/>'
            f'<circle cx="{ocx+R*math.cos(math.radians(256)):.1f}" cy="{ocy+R*math.sin(math.radians(256)):.1f}" r="{H*0.011:.1f}" fill="{NAVY}"/>')
    lbl = txt("CONTACTO · DEPARTAMENTO DE MATERIAS BÁSICAS", W/2, cy + H*0.2, H*0.026, "Medium", INK2, 0.14, anchor="middle")[0]
    body = "\n".join([deco, env, t, lbl, footer_bar(W, H, h=H*0.03)])
    return wrap(W, H, body, "Contacto — basicas@frro.utn.edu.ar")

PIEZAS = {
    "udb-matematica": lambda: udb_banner(1920, 1080, udb("MATEMÁTICA"), [ic_wave, ic_venn, ic_pyr], "U.D.B. Matemática"),
    "udb-fisica": lambda: udb_banner(1920, 1080, udb("FÍSICA"), [ic_atom, ic_pend, ic_lens], "U.D.B. Física"),
    "udb-cultura-e-idiomas": lambda: udb_banner(1920, 1080, udb("CULTURA E", "IDIOMAS"), [ic_globe, ic_book, ic_talk], "U.D.B. Cultura e Idiomas"),
    "udb-legislacion-y-economia": lambda: udb_banner(1920, 1080, udb("LEGISLACIÓN", "Y ECONOMÍA"), [ic_doc, ic_bars, ic_supply], "U.D.B. Legislación y Economía"),
    "laboratorio-informatico": lambda: udb_banner(1920, 1080,
        [("LABORATORIO", 0.62, TEAL, "Bold", 0.03), ("INFORMÁTICO", 1.0, NAVY, "Bold", 0.0),
         ("Y MULTIDISCIPLINAR", 0.62, TEAL, "Bold", 0.03), ("DE MATERIAS BÁSICAS", 0.78, NAVY, "Bold", 0.0)],
        [ic_laptop, ic_flow, ic_db], "Laboratorio Informático y Multidisciplinar de Materias Básicas"),
    "udb-quimica": lambda: udb_banner(1920, 1080, udb("QUÍMICA"), [ic_flask, ic_benz, ic_tubes], "U.D.B. Química"),
    "gabinete-sistemas-de-representacion": lambda: udb_banner(1920, 1080,
        [("GABINETE", 0.62, TEAL, "Bold", 0.03), ("SISTEMAS DE", 1.0, NAVY, "Bold", 0.0), ("REPRESENTACIÓN", 1.0, NAVY, "Bold", 0.0)],
        [ic_iso, ic_compass, ic_square], "Gabinete de Sistemas de Representación"),
    "departamento-portada": portada,
    "contacto": contacto,
}

if __name__ == "__main__":
    import cairosvg
    OUT = sys.argv[1] if len(sys.argv) > 1 else "out"
    for theme in THEMES:
        set_theme(theme)
        os.makedirs(f"{OUT}/svg/{theme}", exist_ok=True); os.makedirs(f"{OUT}/png/{theme}", exist_ok=True)
        for k, fn in PIEZAS.items():
            svg = fn()
            open(f"{OUT}/svg/{theme}/{k}.svg", "w").write(svg)
            cairosvg.svg2png(bytestring=svg.encode(), write_to=f"{OUT}/png/{theme}/{k}.png")
        print(theme, len(PIEZAS))
