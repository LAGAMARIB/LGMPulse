const CACHE_NAME = "lagama-pulse-cache-v6";
const URLS_TO_CACHE = [
    "/",
    "/manifest.json"
];

// Instalação do Service Worker
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(URLS_TO_CACHE))
    );
    self.skipWaiting();
});

// Ativação e limpeza de caches antigos
self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys
                    .filter(key => key !== CACHE_NAME)
                    .map(key => caches.delete(key))
            )
        )
    );
    self.clients.claim();
});

// Intercepta requisições
self.addEventListener("fetch", event => {

    // Navegação (HTML) — garante abertura offline
    if (event.request.mode === "navigate") {
        event.respondWith(
            fetch(event.request)
                .catch(() => caches.match("/"))
        );
        return;
    }

    // Assets — cache first, depois rede
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
