using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniLauncherVersionDisplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI versionText;


    private void Start()
    {
        versionText.text = "ver : " + Application.version;
    }

}
