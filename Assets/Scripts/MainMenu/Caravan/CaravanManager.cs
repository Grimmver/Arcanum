using System.Collections.Generic;
using UnityEngine;

namespace MainMenu.Caravan
{
    public class CaravanManager : MonoBehaviour
    {
        public static CaravanManager Instance { get; private set; }

        [SerializeField] private List<Shelf> shelves;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;


        }

        private void Start()
        {

        }

        private void UpdateShop()
        {

        }
    }
}
