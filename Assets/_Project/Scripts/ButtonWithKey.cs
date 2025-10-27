using UnityEngine;
using UnityEngine.UI;

public class ButtonWithKey : MonoBehaviour
{
    private Button button;
    [SerializeField] private KeyCode keyCode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            button.onClick.Invoke();
        }
    }
}
