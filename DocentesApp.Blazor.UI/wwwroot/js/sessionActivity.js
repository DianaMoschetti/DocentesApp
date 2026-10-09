// [Diana desde v4.0] Detección de actividad del usuario para el timeout por INACTIVIDAD.
// Se carga como módulo ES desde SessionExpirationWatcher (JS interop) una vez por circuito.
//
// Qué hace:
//  - Escucha actividad (click, keydown, scroll, touchstart, mousemove).
//  - Avisa a .NET (OnActivity) a lo sumo cada NOTIFY_THROTTLE_MS para reiniciar el timer de idle.
//  - Mientras hay actividad, pinguea /auth/keepalive a lo sumo cada KEEPALIVE_THROTTLE_MS para
//    renovar la cookie deslizante.
//  - Multi-pestaña: publica la última actividad en localStorage y escucha 'storage', así la
//    actividad en una pestaña reinicia el idle de todas (una pestaña no cierra sesión mientras
//    otra está activa).

const ACTIVITY_EVENTS = ['click', 'keydown', 'scroll', 'touchstart', 'mousemove'];
const STORAGE_KEY = 'docentesapp:lastActivity';
const NOTIFY_THROTTLE_MS = 30 * 1000;        // máx. 1 aviso a .NET cada 30s
const KEEPALIVE_THROTTLE_MS = 2 * 60 * 1000; // máx. 1 keepalive cada 2 min

let dotNetRef = null;
let lastNotify = 0;
let lastKeepalive = 0;
let listenersBound = false;
// Mientras el aviso de inactividad está visible, la actividad pasiva local se ignora,
// así el usuario puede llegar al botón "Seguir conectado" sin reiniciar el timer.
// Solo el botón (forzarActividad) o la actividad de otra pestaña extienden la sesión.
let modoAviso = false;

// Reinicia el timer de idle en .NET y, si la actividad es local, propaga a otras pestañas
// y renueva la cookie. fromOtherTab evita loops (no re-publica ni pinguea).
function notifyActivity(fromOtherTab) {
    const t = Date.now();
    if (t - lastNotify < NOTIFY_THROTTLE_MS) {
        return;
    }
    lastNotify = t;

    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnActivity').catch(() => { });
    }

    if (!fromOtherTab) {
        try { localStorage.setItem(STORAGE_KEY, String(t)); } catch { }
        keepAlive(t);
    }
}

// Pinguea el endpoint solo para que haya un request HTTP real que renueve la cookie deslizante.
// Si devuelve 401, la sesión ya cayó en el server: avisamos a .NET para que el watcher
// redirija por el mismo camino que los demás signos de expiración.
function keepAlive(t) {
    if (t - lastKeepalive < KEEPALIVE_THROTTLE_MS) {
        return;
    }
    lastKeepalive = t;
    fetch('/auth/keepalive', { credentials: 'same-origin' })
        .then(r => {
            if (r.status === 401 && dotNetRef) {
                dotNetRef.invokeMethodAsync('OnUnauthorized').catch(() => { });
            }
        })
        .catch(() => { });
}

function onLocalActivity() {
    // [Diana desde v4.0 OBSOLETO]
    // notifyActivity(false);

    // En modo aviso, la actividad pasiva local no cuenta (ni reinicia timers, ni escribe
    // localStorage, ni pinguea keepalive). Solo el botón "Seguir conectado" extiende la sesión.
    if (modoAviso) {
        return;
    }
    notifyActivity(false);
}

function onStorage(e) {
    if (e.key === STORAGE_KEY && e.newValue) {
        // Actividad en otra pestaña: reiniciar el idle local (saltando el throttle), pero sin
        // re-publicar ni pinguear keepalive (de eso se encarga la pestaña activa).
        lastNotify = 0;
        notifyActivity(true);
    }
}

export function init(dotNetReference) {
    dotNetRef = dotNetReference;
    // El módulo ES se carga una sola vez por documento y su estado sobrevive si el watcher se
    // descarta y se vuelve a crear sin recarga completa. Un watcher nuevo arranca sin aviso
    // visible, así que nunca debe heredar el modo aviso del anterior.
    modoAviso = false;

    if (!listenersBound) {
        ACTIVITY_EVENTS.forEach(ev =>
            window.addEventListener(ev, onLocalActivity, { passive: true }));
        window.addEventListener('storage', onStorage);
        listenersBound = true;
    }

    // Primer latido al arrancar el watcher, para alinear la cookie.
    notifyActivity(false);
}

// La llama el botón "Seguir conectado": sale del modo aviso y cuenta como actividad real,
// salteando los throttles (reinicia timers, escribe localStorage y pinguea keepalive).
export function forzarActividad() {
    modoAviso = false;
    lastNotify = 0;
    lastKeepalive = 0;
    notifyActivity(false);
}

// La llama el watcher para entrar/salir del modo aviso (actividad pasiva local bloqueada).
export function setModoAviso(v) {
    modoAviso = v;
}

export function dispose() {
    if (listenersBound) {
        ACTIVITY_EVENTS.forEach(ev =>
            window.removeEventListener(ev, onLocalActivity));
        window.removeEventListener('storage', onStorage);
        listenersBound = false;
    }
    modoAviso = false;
    dotNetRef = null;
}
