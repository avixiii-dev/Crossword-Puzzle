using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoDialog : Dialog {
    public Text amountText;
    public Text messageText;

    private int amount;

	public void SetAmount(int amount)
    {
        this.amount = amount;
        amountText.text = "x" + amount.ToString();
        messageText.text = string.Format(messageText.text, amount);
    }

    public void Claim()
    {
        Close();
        Sound.instance.PlayButton();
    }

	public override void Close ()
	{
		base.Close ();
		CurrencyController.CreditBalance(amount);
	}
}
