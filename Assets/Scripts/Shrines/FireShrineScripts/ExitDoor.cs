using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene("Main"); // Substitua "Main" pelo nome da sua cena principal
            }
        }
    }
}
