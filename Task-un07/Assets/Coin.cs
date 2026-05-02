using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.instance.AddScore(1);
            Destroy(gameObject);
        }
    }
}