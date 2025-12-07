const CACHE_NAME = "lagama-pulse-cache-v1";
const URLS_TO_CACHE = [
    `/`,
    `/css/site.css`,
    `/js/site.js`,
    `/manifest.json`,
    `/icons/pulse.png`,
    `/icons/xmark.svg`,
    `/Content/fa/css/font-awesome.css`,
    `/Content/fa/css/fontawesome.css`,
    `/Content/fa/css/all.css`,
    // adicione outros arquivos importantes conforme necessário
];

// Instalação do Service Worker
self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => cache.addAll(URLS_TO_CACHE))
    );
    self.skipWaiting();
});

// Ativação e limpeza de caches antigos
self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys.map(key => key !== CACHE_NAME && caches.delete(key))
            )
        )
    );
    self.clients.claim();
});

// Intercepta requisições
self.addEventListener("fetch", event => {
    event.respondWith(
        fetch(event.request)
            .then(response => {
                return response;
            })
            .catch(() =>
                caches.match(event.request).then(resp =>
                    resp || caches.match(`/`)
                )
            )
    );
});


