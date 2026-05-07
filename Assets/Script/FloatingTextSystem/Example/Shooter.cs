using UnityEngine;
using FlyRabbit.FloatingTextSystem;
using TMPro;

public class Shooter : MonoBehaviour
{
    public TMP_FontAsset FontAsset;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Camera camera = Camera.main;

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            Ray ray = camera.ScreenPointToRay(screenCenter);

            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                Enemy enemy = raycastHit.collider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    switch (enemy.Style)
                    {
                        case 0:
                            CritFloating(enemy);
                            break;
                        case 1:
                            GenshinFloating(enemy);
                            break;
                        case 2:
                            Custom1Floating(enemy);
                            break;
                        case 3:
                            Custom2Floating(enemy);
                            break;
                        case 4:
                            TestFloating(enemy);
                            break;
                    }
                }
            }
        }
    }

    private void CritFloating(Enemy enemy)
    {
        string text = UnityEngine.Random.Range(1, 9999).ToString();
        FloatingTextSystem.Instance.CreateFloatingUIText(text, CustomStyle.Crit, enemy.Offset, enemy.transform, FontAsset);
    }

    private void GenshinFloating(Enemy enemy)
    {
        string text = UnityEngine.Random.Range(1, 9999).ToString();
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-0.5f, 0.55f);
        float randomZ = Random.Range(-1f, 1f);
        Color color = Color.white;
        int temp = Random.Range(1, 4);
        switch (temp)
        {
            case 1://火属性
                color = new Color(1f, 0.35f, 0f);
                break;
            case 2://水属性
                color = new Color(0f, 0.78f, 1f);
                break;
            case 3://岩属性
                color = new Color(1f, 0.8f, 0f);
                break;
            default:
                break;
        }
        Vector3 position = new Vector3(randomX, randomY, randomZ) + enemy.transform.position;
        FloatingTextSystem.Instance.CreateFloatingUIText(text, CustomStyle.Genshin, position, null, FontAsset,null, color);
    }
    private void Custom1Floating(Enemy enemy)
    {
        string text = UnityEngine.Random.Range(1, 9999).ToString();
        FloatingTextSystem.Instance.CreateFloatingUIText(text, CustomStyle.Custom1, enemy.Offset, enemy.transform, FontAsset);
    }
    private void Custom2Floating(Enemy enemy)
    {
        string text = UnityEngine.Random.Range(1, 9999).ToString();
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-0.5f, 0.55f);
        float randomZ = Random.Range(-1f, 1f);
        Vector3 position = new Vector3(randomX, randomY, randomZ) + enemy.transform.position;
        FloatingTextSystem.Instance.CreateFloatingUIText(text, CustomStyle.Custom2, position, null, FontAsset);
    }

    private void TestFloating(Enemy enemy)
    {

    }
}
