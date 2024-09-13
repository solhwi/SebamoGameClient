using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;

namespace FirebaseWebGL.Examples.Auth
{
    public class AuthExampleHandler : MonoBehaviour
    {
        public TMP_InputField emailInputField;
        public TMP_InputField passwordInputField;

        public TextMeshProUGUI outputText;

        private void Start()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
                return;
            }
            
            FirebaseAuth.OnAuthStateChanged(gameObject.name, "DisplayUserInfo", "DisplayInfo");
        }

        public void CreateUserWithEmailAndPassword() =>
            FirebaseAuth.CreateUserWithEmailAndPassword(emailInputField.text, passwordInputField.text, gameObject.name, "DisplayInfo", "DisplayErrorObject");

        public void SignInWithEmailAndPassword() =>
            FirebaseAuth.SignInWithEmailAndPassword(emailInputField.text, passwordInputField.text, gameObject.name, "DisplayInfo", "DisplayErrorObject");
        
        public void SignInWithGoogle() =>
            FirebaseAuth.SignInWithGoogle(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        
        public void SignInWithFacebook() =>
            FirebaseAuth.SignInWithFacebook(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        
        public void DisplayData(string data)
        {
            outputText.color = outputText.color == Color.green ? Color.blue : Color.green;
            outputText.text = data;
            Debug.Log(data);
        }

        public void DisplayInfo(string info)
        {
            outputText.color = Color.white;
            outputText.text = info;
            Debug.Log(info);
        }


        public void DisplayError(string error)
        {
            outputText.color = Color.red;
            outputText.text = error;
            Debug.LogError(error);
        }
    }
}