using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    private Button button;
    public Button Button
    {
        get
        {
            if (button == null) button = GetComponent<Button>();
            return button;
        }
    }
}
