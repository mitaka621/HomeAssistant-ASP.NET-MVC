const toastLiveExample = document.getElementById('liveToast')

if (toastLiveExample) {
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLiveExample)
        toastBootstrap.show()
    
}