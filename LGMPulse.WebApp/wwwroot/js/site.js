// Registrar Service Worker
if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register("/sw.js")
        .catch(err => console.error("Erro ao registrar Service Worker:", err));
}

