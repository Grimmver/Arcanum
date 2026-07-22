using UnityEngine;

namespace UI
{
    public class CoinUI : MoneyUI
    {
        private const string COIN_PREF = "Coin";
        protected override string PrefKey => COIN_PREF;
    }
}
