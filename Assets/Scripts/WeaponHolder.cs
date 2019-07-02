using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private bool isWeaponActive;

    private void Start()
    {
        isWeaponActive = false;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (isWeaponActive)
        {
            DrawLine(gameObject.transform.position, new Vector3(ray.origin.x, ray.origin.y, 0), Color.red, 0.3f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWeaponActive = !isWeaponActive;
        }

    }
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        Destroy(myLine, duration);
    }
}
