mergeInto(LibraryManager.library, {

    SignInWithGoogle: function (objectName, callback, fallback) {
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        (async () => {
            const { initializeApp } = await import('https://www.gstatic.com/firebasejs/10.13.1/firebase-app.js');
            const { getAuth, signInWithPopup, GoogleAuthProvider } = await import('https://www.gstatic.com/firebasejs/10.13.1/firebase-auth.js');

            // Firebase 앱 초기화
            const firebaseConfig = {
                apiKey: "AIzaSyAHm55JW_S4i_D-C3QUcpu4SIjlwKpSLtw",
                authDomain: "sebamogame.firebaseapp.com",
                projectId: "sebamogame",
                storageBucket: "sebamogame.appspot.com",
                messagingSenderId: "659799069037",
                appId: "1:659799069037:web:11b3d97618697215d58a64",
                measurementId: "G-W1KECLMW4V",
                googleAppId: "659799069037-t3mjqgrq9iq4ohbmfsri41h11ruc7r6e.apps.googleusercontent.com"
            };
    
            const app = initializeApp(firebaseConfig);
            const auth = getAuth(app);

            // Google 제공자 초기화
            const provider = new GoogleAuthProvider();

            // 팝업을 통한 Google 로그인 시도
            signInWithPopup(auth, provider)
                .then((result) => {
                    const user = result.user;
                    console.log("Google Sign-In successful! User: " + user.email);
                    // 성공 시 Unity로 성공 메시지 전달
                    SendMessage(parsedObjectName, parsedCallback, user.email);
                })
                .catch((error) => {
                    console.error("Google Sign-In error: " + error.message);
                    // 실패 시 Unity로 에러 메시지 전달
                    SendMessage(parsedObjectName, parsedFallback, error.message);
                });
        })();
    }
});
