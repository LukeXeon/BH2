using UnityEngine;

public class TestOpenUrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var tf = (RectTransform) transform;
        var rect = tf.rect;
        var position = tf.position;
        var v2 = RectTransformUtility.WorldToScreenPoint(Camera.current, new Vector3(0, position.y + rect.height / 2));
        Debug.Log(v2);
        Debug.Log(Screen.height);
    }


    public void onTest()
    {
    }

    private void OnDestroy()
    {
    }
}