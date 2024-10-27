using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class DisclaimerScreen : MonoBehaviour
    {
        private Fader _fader;
        private TextMeshProUGUI _disclaimerText;

        private void Awake()
        {
            _fader = GetComponentInChildren<Fader>();
            _fader.SetDefaultColor(Color.black);
        
            _disclaimerText = GameObject.Find("DisclaimerText").GetComponent<TextMeshProUGUI>();
            _disclaimerText.SetText("This is a Touhou Project derivative work.");
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(_fader.FadeIn(.5f));
            yield return new WaitForSeconds(2f);
            StartCoroutine(_fader.FadeOut(.5f));
            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene("Main");
        }
    }
}
