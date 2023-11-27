using UnityEngine;
using UnityEngine.UI;

public class UIImageFollower : MonoBehaviour
{
    public Image follower;
    public Canvas canvas;
    
    // Update is called once per frame
    void Update()
    {
        //Follow cursor with an image, remember to zero out image anchors, pivot and positions. Select Anchor Preset for bottom left.
        follower.rectTransform.anchoredPosition = Input.mousePosition / canvas.transform.localScale.x - new Vector3(80, 80, 0);
    }
}
