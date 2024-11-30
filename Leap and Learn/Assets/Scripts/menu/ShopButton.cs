using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButton : MonoBehaviour
{
    public void StartShop()
    {
        SceneManager.LoadScene("Shop");
    }
}
