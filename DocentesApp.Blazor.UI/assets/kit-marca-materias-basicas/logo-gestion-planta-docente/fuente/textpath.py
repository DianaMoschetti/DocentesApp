from fontTools.ttLib import TTFont
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.pens.transformPen import TransformPen
F="/usr/share/fonts/truetype/google-fonts/Poppins-%s.ttf"
_c={}
def font(w):
    if w not in _c: _c[w]=TTFont(F%w)
    return _c[w]
def text_path(s, x, y, size, weight="Bold", tracking=0.0):
    """Return (d, width). tracking in em."""
    f=font(weight); gs=f.getGlyphSet(); cmap=f.getBestCmap(); upm=f['head'].unitsPerEm
    sc=size/upm; pen=SVGPathPen(gs); cx=0
    for ch in s:
        g=cmap.get(ord(ch))
        if g is None: continue
        tp=TransformPen(pen,(sc,0,0,-sc,x+cx,y))
        gs[g].draw(tp)
        cx+=gs[g].width*sc+tracking*size
    return pen.getCommands(), cx-tracking*size
