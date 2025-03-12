using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public int choosenSkinId;
    public static SkinManager inst;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (inst == null)
        {
            inst = this;
        }
        else
        {
            // If there is more than one GameManager it will delete one of them.
            Destroy(gameObject);
        }
    }

    public void SetSkinId(int id)
    {
        choosenSkinId = id;
    }

    public int GetSkinId() => choosenSkinId;
}
