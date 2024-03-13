using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color originalDotColor;

    private void Start()
    {
        Cursor.visible = false; // 크로스헤어의 start에서 커서를 숨기기 때문에 더 게임 중 어떤 이유로 커서가 생기면 그 다음부턴 사라지지 않음
        originalDotColor = dot.color;
    }
    void Update()
    {
        transform.Rotate (Vector3.forward * -40 *  Time.deltaTime);    
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
