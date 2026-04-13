
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LightChange : MonoBehaviour
{

    private Light directionalLight;
    [SerializeField] private Image blackScreen;
    [SerializeField] private Text text;
    [SerializeField] private InputActionReference input;

    private bool isSimulating = false;

    public Color[] wavelengths = new Color[]
    {
        new Color(131, 0, 181), //400 nm
        new Color(0, 192, 255), // 475 nm
        new Color(0, 255, 146), //500 nm
        new Color(74, 255, 0), //525 nm
        new Color(163, 255, 0), // 550 nm
        new Color(225, 255, 0), // 570 nm
        new Color(255, 190, 0), // 600 nm
        new Color(255, 79, 0), //630 nm
        new Color(255, 0, 0) //700nm

    };


    private void OnEnable()
    {
        input.action.Enable();
        input.action.performed += StartSimulation;
    }

    private void OnDisable()
    {
        input.action.Disable();
        input.action.performed -= StartSimulation;
    }

    private void StartSimulation(InputAction.CallbackContext context)
    {
        if (isSimulating == false)
        {
            text.enabled = false;
            isSimulating= true;
            StartCoroutine(ColorSwap());
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        directionalLight = GetComponent<Light>();
        if(directionalLight == null || blackScreen == null || text == null ) { return; }
        
    }

    IEnumerator ColorSwap()
    {
        int count = 0;
        print("ColorSwap!");
        while (count < wavelengths.Length)
        {
            blackScreen.enabled = true;
            yield return new WaitForSeconds((float)0.5);
            blackScreen.enabled = false;

            directionalLight.color = wavelengths[count % wavelengths.Length];
            print(directionalLight.color);
            count++;
            yield return new WaitForSeconds(5);
        }

        text.enabled = true;
        blackScreen.enabled = true;
        isSimulating = false;
    }
}
