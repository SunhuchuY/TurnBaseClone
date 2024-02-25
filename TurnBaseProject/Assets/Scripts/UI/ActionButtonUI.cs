using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private Image selectedImage;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;   
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() => 
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedImage()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedImage.gameObject.SetActive(selectedAction == baseAction);
    }
}
