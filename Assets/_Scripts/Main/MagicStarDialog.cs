using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicStarDialog : Dialog {

    public Text message;

    protected override void Start()
    {
        message.text = string.Format(message.text, ConfigController.Config.showLetterCost);
        base.Start();
    }
}
