using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] int maxIndex;

    public int index;
    public AudioSource audioSource;
    public bool isButtonSelected;
    bool keyDown;
    float horizontal;
    float vertical;

    void Start() => audioSource = GetComponent<AudioSource>();

    // Navigate through the menu button indices
    void Update()
    {
        if (isButtonSelected)
        {
            return;
        }
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical != 0 || horizontal != 0)
        {
            // Cannot hold button down and scroll through menus
            if (!keyDown)
            {
                if (vertical < 0 || horizontal > 0)
                {
                    if(index < maxIndex)
                        index++;
                    else
                        index = 0;
                }
                else if (vertical > 0 || horizontal < 0)
                {
                    if (index > 0)
                        index--;
                    else
                        index = maxIndex;
                }
                keyDown = true;
            }
        }
        else
            keyDown = false;
    }
}
