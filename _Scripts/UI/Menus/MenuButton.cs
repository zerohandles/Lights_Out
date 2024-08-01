using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorFunctions;
    [SerializeField] int thisIndex;
    Button button;

    private void Start() => this.button = GetComponent<Button>();

    // Wait for input and trigger the pressed state of the button
    void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("Selected", true);
            if (Input.GetAxisRaw("Submit") == 1)
            {
                animator.SetBool("Pressed", true);
                button.onClick.Invoke();
            }
            else if (animator.GetBool("Pressed"))
            {
                animator.SetBool("Pressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else
        {
            animator.SetBool("Selected", false);
            animator.SetBool("Normal", true);
        }
    }
}
