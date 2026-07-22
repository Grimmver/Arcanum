using UnityEngine;

namespace UI
{
    public class CrystalUI : MoneyUI
    {
        private const string CRYSTAL_PREF = "Crystal";
        protected override string PrefKey => CRYSTAL_PREF;
    }
}
